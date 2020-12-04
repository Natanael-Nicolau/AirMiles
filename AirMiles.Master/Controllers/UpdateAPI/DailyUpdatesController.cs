using AirMiles.Master.Helpers.DailyUpdates;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirMiles.Master.Controllers.UpdateAPI
{

    [Route("UpdateAPI/[controller]/[action]")]
    [ApiController]
    public class DailyUpdatesController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMileRepository _mileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDailyUpdateHelper _dailyUpdateHelper;

        public DailyUpdatesController(
            IClientRepository clientRepository,
            IMileRepository mileRepository,
            IUserRepository userRepository,
            ITicketRepository ticketRepository,
            IPartnerRepository partnerRepository,
            ITransactionRepository transactionRepository,
            IDailyUpdateHelper dailyUpdateHelper)
        {
            _clientRepository = clientRepository;
            _mileRepository = mileRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _partnerRepository = partnerRepository;
            _transactionRepository = transactionRepository;
            _dailyUpdateHelper = dailyUpdateHelper;
        }

        public async Task<bool> DailyRevision()
        {
            bool isSuccess = await UpdateTickets();
            if (!isSuccess)
            {
                return false;
            }

            //Check if client is eligeable for status upgrade
            isSuccess = await UpgradeClients();
            if (!isSuccess)
            {
                return false;
            }


            await _ticketRepository.DeleteOldTickets();
            await _mileRepository.DeleteExpiredMilesAsync();
            return true;
        }

        private async Task<bool> UpgradeClients()
        {
            var clients = _clientRepository.GetAllWithUsers().ToList();

            foreach (Client client in clients)
            {
                var clientStatus = await _userRepository.GetClientStatusRoleAsync(client.User.Email);
                var totalMiles = _mileRepository.GetClientTotalStatusMiles(client.Id);

                if (clientStatus == "Basic" && (totalMiles >= 30000 || client.TotalFlights >= 25))
                {
                    await _userRepository.RemoveFromRole(client.User, "Basic");
                    await _userRepository.AddUsertoRoleAsync(client.User, "Silver");
                }
                else if (clientStatus == "Silver" && (totalMiles >= 70000 || client.TotalFlights >= 50))
                {
                    await _userRepository.RemoveFromRole(client.User, "Silver");
                    await _userRepository.AddUsertoRoleAsync(client.User, "Gold");
                }
            }

            return true;
        }

        /// <summary>
        /// Tasks that run on the first of the month to update the status of last month users
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MonthlyRevision()
        {
            //get list with all users that update this month
            var clients = _clientRepository.GetAllWithUsers()
                .Where(c => c.RevisionMonth == (DateTime.Now.Month - 1))
                .ToList();



            foreach (var client in clients)
            {
                //calculate if user maintains or drops a level
                var user = await _userRepository.GetUserByIdAsync(client.UserId);
                if (user != null)
                {
                    var clientMiles = _mileRepository.GetClientTotalStatusMiles(client.Id);
                    var clientStatus = await _userRepository.GetClientStatusRoleAsync(user.Email);
                    if (clientStatus == null)
                    {
                        await _userRepository.AddUsertoRoleAsync(user, "Basic");
                    }

                    if (clientStatus == "Gold")
                    {
                        if (!(clientMiles >= 50000 || client.TotalFlights >= 40 ))
                        {
                            //Delete this line if you don't want him to drop 2 levels
                            clientStatus = "Silver";
                            await _userRepository.RemoveFromRole(user, "Gold");
                            await _userRepository.AddUsertoRoleAsync(user, "Silver");
                        }
                    }
                    if(clientStatus == "Silver")
                    {
                        if (!(clientMiles >= 20000 || client.TotalFlights >= 15))
                        {
                            clientStatus = "Basic";
                            await _userRepository.RemoveFromRole(user, "Silver");
                            await _userRepository.AddUsertoRoleAsync(user, "Basic");
                        }
                    }

                    //TODO: remove range status miles                 
                    await _mileRepository.ResetClientStatusMiles(client.Id);


                    client.BoughtMiles = 0;
                    client.ConvertedMiles = 0;
                    client.ExtendedMiles = 0;
                    client.TransferedMiles = 0;
                    client.TotalFlights = 0;

                    await _clientRepository.UpdateAsync(client);

                }
            }

            return true;
        }

        private async Task<bool> UpdateTickets()
        {
            try
            {
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://10.147.17.1:50000"),
                };

                string url = $"/todayTickets";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                List<TicketUpdateModel> list = JsonConvert.DeserializeObject<List<TicketUpdateModel>>(result);

                //filter tickets with clients
                list.RemoveAll(t => string.IsNullOrEmpty(t.ClientId));


                foreach (var ticket in list)
                {
                    int.TryParse(ticket.ClientId, out int clientId);

                    //Check if client exists
                    var client = await _clientRepository.GetByIdAsync(clientId);
                    if (client != null)
                    {
                        client.User = await _userRepository.GetUserByIdAsync(client.UserId);
                        if (client.User != null)
                        {
                            var clientStatus = await _userRepository.GetClientStatusRoleAsync(client.User.Email);


                            //calculate number of miles to be credited
                            var baseMiles = await _dailyUpdateHelper.CalculateTicketMilesAsync(ticket);


                            //Status Modifier
                            double statusModifier = 1;
                            if (clientStatus == "Silver")
                            {
                                statusModifier = 1.25;
                            }
                            else if (clientStatus == "Gold")
                            {
                                statusModifier = 1.50;
                            }

                            //Region Modifier
                            double regionModifier = _dailyUpdateHelper.CalculateRegionModifier(ticket.StartRegion, ticket.EndRegion, ticket.FlightClass);

                            //Update client miles
                            int milesToBeCredited = Convert.ToInt32(Math.Truncate(baseMiles * statusModifier * regionModifier));


                            var newBonusMile = new Mile
                            {
                                ClientId = client.Id,
                                ExpirationDate = DateTime.Now.AddYears(3),
                                IsAproved = true,
                                IsDeleted = false,
                                MilesTypeId = 2,
                                Qtd = milesToBeCredited
                            };
                            await _mileRepository.CreateAsync(newBonusMile);


                            var flightCompany = await _partnerRepository.GetByNameAsync(ticket.FlightCompanyName);
                            if (flightCompany != null)
                            {
                                if (flightCompany.IsStarAlliance)
                                {
                                    var newStatusMile = new Mile
                                    {
                                        ClientId = client.Id,
                                        ExpirationDate = DateTime.Now.AddYears(3),
                                        IsAproved = true,
                                        IsDeleted = false,
                                        MilesTypeId = 1,
                                        Qtd = milesToBeCredited
                                    };
                                    await _mileRepository.CreateAsync(newBonusMile);
                                }
                            }

                            //Update Client transactions
                            var transaction = new Transaction
                            {
                                ClientID = clientId,
                                Description = "Flight Credit",
                                IsAproved = true,
                                IsCreditCard = false,
                                IsDeleted = false,
                                Value = milesToBeCredited,
                                TransactionDate = DateTime.Now
                            };

                            await _transactionRepository.CreateAsync(transaction);
                        }
                    }
                }                

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        

    }
}