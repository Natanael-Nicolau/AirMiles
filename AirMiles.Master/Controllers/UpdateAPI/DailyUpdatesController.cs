using AirMiles.Master.Helpers.DailyUpdates;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirMiles.Master.Controllers.UpdateAPI
{

    [Route("updates/[controller]")]
    public class DailyUpdatesController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDailyUpdateHelper _dailyUpdateHelper;

        public DailyUpdatesController(
            IClientRepository clientRepository,
            IUserRepository userRepository,
            IDailyUpdateHelper dailyUpdateHelper)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _dailyUpdateHelper = dailyUpdateHelper;
        }

        public async Task<bool> UpdateTickets()
        {
            try
            {
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://10.147.17.1:50000"),
                };

                string url = $"/list";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                List<TicketUpdateModel> list = JsonConvert.DeserializeObject<List<TicketUpdateModel>>(result);

                //filter tickets with clients
                list.RemoveAll(t => string.IsNullOrEmpty(t.ClientId));


                foreach (var ticket in list)
                {
                    int clientId;
                    int.TryParse(ticket.ClientId, out clientId);

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

                            if (clientStatus == "Silver")
                            {
                                baseMiles = Convert.ToInt32(Math.Truncate(baseMiles * 1.25));
                            }
                            if (clientStatus == "Gold")
                            {
                                baseMiles = Convert.ToInt32(Math.Truncate(baseMiles * 1.50));
                            }






                            //update client tickets
                            //Update client miles
                            //Check if client is eligeable for status upgrade


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