using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.FrontOffice.Helpers;
using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirMiles.FrontOffice.Controllers
{
    public class MilesController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMileRepository _mileRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public MilesController(IUserRepository userRepository, IMailHelper mailHelper, IClientRepository clientRepository, IConverterHelper converterHelper, ITransactionRepository transactionRepository, IMileRepository mileRepository, IImageHelper imageHelper, IFlightRepository flightRepository, ITicketRepository ticketRepository)
        {
            _userRepository = userRepository;
            _mailHelper = mailHelper;
            _clientRepository = clientRepository;
            _converterHelper = converterHelper;
            _transactionRepository = transactionRepository;
            _mileRepository = mileRepository;
            _imageHelper = imageHelper;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }

        #region Miles&Go

        [Authorize]
        public async Task<IActionResult> BalanceMovements()
        {
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            if (client == null)
            {
                return NotFound();
            }

            var transactions = _transactionRepository.GetAllByClientId(client.Id).OrderByDescending(t => t.TransactionDate).ToList();


            var model = _converterHelper.ToBalanceMovementsViewModel(transactions);

            return View(model);
        }

        public IActionResult DownloadBalanceMovementsPDF()
        {
            var data = Request.Query["info"].ToString();

            var lines = data.Split(",");

            var headers = Request.Query["headers"].ToString();

            var columns = headers.Split(",");

            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(ms))
                {
                    writer.SetCloseStream(false);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    Table table = new Table(new float[10]).UseAllAvailableWidth();


                    foreach (var value in columns)
                    {
                        Cell header = new Cell().Add(new Paragraph(value));
                        header.SetTextAlignment(TextAlignment.CENTER);
                        header.SetBackgroundColor(new DeviceRgb(0, 194, 146));
                        table.AddHeaderCell(header);
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        Cell cell = new Cell().Add(new Paragraph(lines[i]));
                        cell.SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(cell);

                        if ((i + 1) % columns.Length == 0 && i != 0)
                        {
                            table.StartNewRow();
                        }
                    }

                    document.Add(table);
                    document.Close();

                    byte[] file = ms.ToArray();
                    ms.Write(file, 0, file.Length);
                    ms.Position = 0;

                    return File(file, "application/pdf", "Balance&Movements.pdf");
                }
            }
        }

        [Authorize]
        public IActionResult BuyTicket()
        {
            var flights = _flightRepository.GetAllWithAirportsAndPartners().Where(f => f.IsAproved);

            var model = new List<BuyTicketViewModel>();

            foreach (var flight in flights)
            {
                var ticket = new BuyTicketViewModel
                {
                    Id = flight.Id,
                    StartAirport = flight.StartAirport.Name,
                    EndAirport = flight.EndAirport.Name,
                    Price = flight.BaseMilesPrice,
                    Company = flight.FlightCompany.Name,
                    FlightStart = flight.FlightStart,
                    FlightEnd = flight.FlightEnd
                };

                model.Add(ticket);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BuyTicket(int flightId, string price, int flightClassId)
        {
            if (flightId != 0 || string.IsNullOrEmpty(price) || flightClassId != 0)
            {

                int newPrice = Convert.ToInt32(price);

                // Gets the client by Email
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                // Gets the user by Email
                var user = await _userRepository.GetUserByEmailAsync(this.User.Identity.Name);


                // Gets a list of the client Bonus Miles
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);

                int bonusMiles = 0;

                foreach (var mile in clientMiles)
                {
                    bonusMiles += mile.Qtd;
                }

                // Checks if client has enough miles to buy the selected Ticket
                if (bonusMiles < newPrice)
                {
                    return StatusCode(400, "You dont have enough miles to purchase this ticket!");
                }

                var selectedFlight = _flightRepository.GetAllWithAirportsAndPartners().Where(f => f.IsAproved && f.Id == flightId).FirstOrDefault();
                if (selectedFlight == null)
                {
                    return StatusCode(404, "Flight Not Found!");
                }


                var ticket = new Ticket
                {
                    ClientId = client.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FlightId = flightId,
                    FlightClassId = flightClassId,
                    Price = newPrice,
                    IsAproved = false,
                    IsDeleted = false,
                };

                // Creates the Ticket in the DataBase
                try
                {
                    await _ticketRepository.CreateAsync(ticket);
                }
                catch (Exception)
                {
                    return StatusCode(520, "Unexpected DataBase Error");
                }

                return StatusCode(200, "Ticket awaiting approval!");
            }

            return StatusCode(420, "There was an error processing your request!");
        }

        public async Task<IActionResult> TicketIndex()
        {
            // Gets the client by Email
            var user = await _userRepository.GetUserByEmailAsync(this.User.Identity.Name);

            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            var tickets = _ticketRepository.GetAll().Where(t => t.IsAproved && t.ClientId == client.Id).ToList();

            foreach(var ticket in tickets)
            {
                ticket.Flight = await _flightRepository.GetByIdWithAirportsAndPartnersAsync(ticket.FlightId);

                ticket.FlightClass = await _flightRepository.GetFlightClassByIdAsync(ticket.FlightClassId);
            }

            var model = _converterHelper.ToTicketIndexViewModel(user, tickets);

            return View(model);
        }
        public IActionResult DownloadTicketPDF()
        {
            var data = Request.Query["info"].ToString();

            var lines = data.Split(",");

            var headers = Request.Query["headers"].ToString();

            var columns = headers.Split(",");

            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(ms))
                {
                    writer.SetCloseStream(false);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    Table table = new Table(new float[10]).UseAllAvailableWidth();


                    for (int i = 0; i < columns.Length - 1; i++)
                    {
                        Cell header = new Cell().Add(new Paragraph(columns[i]));
                        header.SetTextAlignment(TextAlignment.CENTER);
                        header.SetBackgroundColor(new DeviceRgb(0, 194, 146));
                        table.AddHeaderCell(header);
                    }

                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        Cell cell = new Cell().Add(new Paragraph(lines[i]));
                        cell.SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();

                    byte[] file = ms.ToArray();
                    ms.Write(file, 0, file.Length);
                    ms.Position = 0;

                    return File(file, "application/pdf", "Ticket.pdf");
                }
            }
        }

        #endregion

        #region MilesStore

        public IActionResult BuyMiles()
        {
            // Generates a new instance of BuyMilesViewModel
            var models = new List<BuyMilesViewModel>();

            // Creates a new integer to use in the while 
            var i = 1;

            // Assigns values to the new BuyMilesViewModel
            while (i < 11)
            {
                var mile = new BuyMilesViewModel
                {
                    Amount = 2000 * i,
                    Price = 70 * i,
                    Selected = false
                };

                i++;

                models.Add(mile);
            }

            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> BuyMiles(IList<BuyMilesViewModel> models)
        {
            if (ModelState.IsValid)
            {
                // Selects the value selected by the client
                var model = models.Where(m => m.Selected).FirstOrDefault();

                // Returns an error in case no value was selected
                if (model == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Please select an amount");
                    return View(models);
                }

                // Gets the client by Email
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                // Adds the value of the selected value amount with the pre-existing client Bought Miles and assigns it to a variable
                var boughtMiles = model.Amount + client.BoughtMiles;

                // Checks if the value is bigger than 20000 and if true, returns an error
                if (boughtMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only buy a maximum of 20.000 Miles per Year");

                    return View(models);
                }

                // Converts the selected value to a Mile Object
                var mile = _converterHelper.ToMile(model, client.Id, 1);

                // Converts the selected value and the new Mile Object to a Transaction Object 
                var transaction = _converterHelper.ToTransaction(model, mile);

                // Creates a new Mile in the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);

                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                // Creates a new Transaction in the DataBase
                try
                {
                    await _transactionRepository.CreateAsync(transaction);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                // Updates the client
                try
                {
                    client.BoughtMiles = boughtMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }
                // Returns a message if the operation was valid
                ViewBag.Message = "Your purchase was successful!";
            }
            return View(models);
        }

        public IActionResult TransferMiles()
        {
            // Generates a new instance of TransferMilesViewModel
            var model = new TransferMilesViewModel
            {
                Amount = 2000,
                GiftedClientID = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TransferMiles(TransferMilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Gets the current Client
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                // Gets the client to gift Miles
                var giftedClient = await _clientRepository.GetByIdAsync(Convert.ToInt32(model.GiftedClientID));

                // Checks if the giftedClient exists
                if (giftedClient == null)
                {
                    this.ModelState.AddModelError(string.Empty, "The selected Client does not exist. Please try again!");
                    return View(model);
                }

                //Checks if the maximum value of transfered miles has been reached
                var transferMiles = model.Amount + client.TransferedMiles;

                if (transferMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only transfer a maximum of 20.000 Miles per Year");

                    return View(model);
                }

                // Gets a list of the client Bonus Miles
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);

                // Creates a new var to hold the client Total Bonus Miles
                var clientTotalMiles = _mileRepository.GetClientTotalBonusMiles(client.Id);

                // Checks if the client has enough Miles to perform the Transfer Operation
                if (clientTotalMiles < model.Amount)
                {
                    this.ModelState.AddModelError(string.Empty, "You dont have the necessary Miles to perform this transfer. Please try again with a lower value!");

                    return View(model);
                }

                // Creates a new Mile Object for the giftedClient
                var mile = _converterHelper.ToMile(giftedClient.Id, model.Amount, 1);

                // Creates the Transaction on the giftedClient end
                var receivedMiles = _converterHelper.ToTransaction(giftedClient.Id, mile.Qtd);

                // Creates the Transaction on the Client that will transfer miles end
                // IMPORTANT NOTE:
                // SUBTRACT THE MILES QUANTITY
                var giftedMiles = _converterHelper.ToTransaction(giftedClient.Id, -mile.Qtd);

                // Creates a new Mile on the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                // Creates two transaction on the Database
                // One for the gifter and another for the receiver
                try
                {
                    await _transactionRepository.CreateAsync(receivedMiles);
                    await _transactionRepository.CreateAsync(giftedMiles);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                // Transfers the Miles
                var transferSuccess = await _mileRepository.SpendMilesAsync(clientMiles, transferMiles);

                if (!transferSuccess)
                {

                    this.ModelState.AddModelError(string.Empty, "There was a critical error with the transfer algorithm. Please contact the Administrator as soon as possible");
                    return View(model);
                }

                // Updates the client Transfer Miles on the DataBase
                try
                {
                    client.TransferedMiles = transferMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                ViewBag.Message = "Your transfer was successful!";
                return View();
            }

            this.ModelState.AddModelError(string.Empty, "Please select a valid Client Id");
            return View(model);
        }

        [Authorize]
        public IActionResult ExtendMiles()
        {
            // Generates a new instance of ProlongMilesViewModel
            var model = new ExtendMilesViewModel
            {
                Amount = 2000
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ExtendMiles(ExtendMilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Gets the current Client
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                //Checks if the maximum value of transfered miles has been reached
                var prolongMiles = model.Amount + client.ExtendedMiles;

                if (prolongMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only extend a maximum of 20.000 Miles per Year");

                    return View(model);
                }

                // Gets a list of the client Bonus Miles
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);

                // Creates a new Mile Object for the giftedClient
                var newDate = clientMiles.OrderBy(m => m.ExpirationDate).FirstOrDefault().ExpirationDate.AddYears(1);
                var mile = _converterHelper.ToMile(model, client.Id, newDate);

                var extendedMiles = _converterHelper.ToTransaction(model, mile);


                // Creates a transaction on the Database
                try
                {
                    await _transactionRepository.CreateAsync(extendedMiles);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your extension.Please try again later");
                    return View(model);
                }


                // Extends the Miles
                var extendSuccess = await _mileRepository.SpendMilesAsync(clientMiles, model.Amount);

                if (!extendSuccess)
                {

                    this.ModelState.AddModelError(string.Empty, "There was a critical error with the extension algorithm. Please contact the Administrator as soon as possible");
                    return View(model);
                }

                // Creates a new Mile on the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                // Updates the client Prolonged Miles on the DataBase
                try
                {
                    client.ExtendedMiles = prolongMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                ViewBag.Message = "Your extension of miles was successful!";
                return View(model);

            }

            // Generates a new instance of ExtendMilesViewModel
            var returnModel = new ExtendMilesViewModel
            {
                Amount = 2000
            };

            return View(model);
        }

        [Authorize]
        public IActionResult ConvertMiles()
        {
            // Generates a new instance of ProlongMilesViewModel
            var model = new ConvertMilesViewModel
            {
                BonusAmount = 2000,
                StatusAmount = 1000
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConvertMiles(ConvertMilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);
                if (client == null)
                {
                    return NotFound();
                }

                //Checks if the maximum value of transfered miles has been reached
                var convertMiles = model.BonusAmount + client.ExtendedMiles;

                if (convertMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only convert a maximum of 20.000 Miles per Year");

                    return View(model);
                }


                //Checks if client is eligeable for mile conversion
                int totalStatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id);
                var status = await _userRepository.GetClientStatusRoleAsync(this.User.Identity.Name);
                if (string.IsNullOrEmpty(status))
                {
                    return NotFound();
                }

                if ((status == "Basic" && (totalStatusMiles < 25000 || totalStatusMiles >= 30000)) || (status == "Silver" && (totalStatusMiles < 65000 || totalStatusMiles >= 70000)) || status == "Gold")
                {
                    ViewBag.Message = "You aren't eligeable for mile conversion";
                    return View(model);
                }

                // Gets a list of the client Bonus Miles
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);


                // Creates a new Mile Object for the client
                var mile = _converterHelper.ToMile(model, client.Id);

                var convertedMiles = _converterHelper.ToTransaction(model, mile);


                // Creates a transaction on the Database
                try
                {
                    await _transactionRepository.CreateAsync(convertedMiles);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your conversion. Please try again later");
                    return View(model);
                }


                // Converts the Miles
                var convertSuccess = await _mileRepository.SpendMilesAsync(clientMiles, model.BonusAmount);
                if (!convertSuccess)
                {

                    this.ModelState.AddModelError(string.Empty, "There was a critical error with the conversion algorithm. Please contact the Administrator as soon as possible");
                    return View(model);
                }

                // Creates a new Mile on the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                // Updates the client Converted Miles on the DataBase
                try
                {
                    client.ConvertedMiles = convertMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                ViewBag.Message = "Your conversion of miles was successful!";
            }
            return View(model);
        }

        #endregion
    }
}
