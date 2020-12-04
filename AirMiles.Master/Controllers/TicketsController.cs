using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Models.Tickets;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AirMiles.Master.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMileRepository _mileRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TicketsController(
            ITicketRepository ticketRepository,
            IMileRepository mileRepository,
            IClientRepository clientRepository,
            ITransactionRepository transactionRepository)
        {
            _ticketRepository = ticketRepository;
            _mileRepository = mileRepository;
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
        }

        public IActionResult ApprovalIndex()
        {
            var model = _ticketRepository.GetAllWithFlightInfo()
                .Where(t => !t.IsAproved)
                .Select(t => new ApprovalIndexViewModel
                {
                    Id = t.Id,
                    StartIATA = t.Flight.StartAirport.IATA,
                    EndIATA = t.Flight.EndAirport.IATA,
                    StartTime = t.Flight.FlightStart,
                    Class = t.FlightClass.Description
                });

            return View(model);
        }


        public async Task<IActionResult> Aprove(int? id, string seat)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var ticket = await _ticketRepository.GetByIdAsync(id.Value);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Seat = seat.ToUpper();
                ticket.IsAproved = true;

                await _ticketRepository.UpdateAsync(ticket);

                //Spend miles
                var client = await _clientRepository.GetByIdAsync(ticket.ClientId);
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);


                await _mileRepository.SpendMilesAsync(clientMiles, Convert.ToInt32(ticket.Price));

                //create transaction
                var transaction = new Transaction
                {
                    ClientID = client.Id,
                    Description = "Ticket",
                    IsAproved = true,
                    IsDeleted = false,
                    IsCreditCard = false,
                    Value = -Convert.ToInt32(ticket.Price),
                    TransactionDate = DateTime.Now,
                };

                await _transactionRepository.CreateAsync(transaction);

                return StatusCode(200, "Ticket Aproved");
            }
            catch (Exception)
            {

                return StatusCode(520, "Unkown Error");
            }
            
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var ticket = await _ticketRepository.GetByIdAsync(id.Value);
                if (ticket == null)
                {
                    return NotFound();
                }

                await _ticketRepository.DeleteAsync(ticket);
                return StatusCode(200, "Ticket Deleted");
            }
            catch (Exception)
            {
                return StatusCode(520, "Unkown Error");
            }
        }


    }
}
