using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Flights;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Controllers
{
    [Authorize]
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IConverterHelper _converterHelper;

        public FlightsController(
            IFlightRepository flightRepository,
            IAirportRepository airportRepository,
            IPartnerRepository partnerRepository,
            IConverterHelper converterHelper)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
            _partnerRepository = partnerRepository;
            _converterHelper = converterHelper;
        }

        public IActionResult Index()
        {
            var list = _flightRepository.GetAllWithAirportsAndPartners()
                .Where(f => f.IsAproved == true)
                .Select(f => new IndexViewModel
                {
                    Id = f.Id,
                    StartAirportName = f.StartAirport.Name,
                    EndAirportName = f.EndAirport.Name,
                    FlightStart = f.FlightStart,
                    FlightCompanyName = f.FlightCompany.Name,
                    IsAproved = f.IsAproved
                });
            return View(list);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                StartAirports = _airportRepository.GetStartingAirports(),
                EndAirports = _airportRepository.GetEndAirports(0),
                FlightCompanies = _partnerRepository.GetAllFlightCompanies(),
                FlightStart = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "yyyy-MM-dd HH:mm", null),
                FlightEnd = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "yyyy-MM-dd HH:mm", null).AddHours(1),
            };
            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.FlightStart <= DateTime.Now)
                {
                    ModelState.AddModelError("FlightStart", "Please select a Date after today.");
                    model.StartAirports = _airportRepository.GetStartingAirports();
                    model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
                    model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
                    return View(model);
                }
                if (model.FlightEnd <= model.FlightStart)
                {
                    ModelState.AddModelError("FlightEnd", "You can't end a flight before it starts.");
                    model.StartAirports = _airportRepository.GetStartingAirports();
                    model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
                    model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
                    return View(model);
                }

                var newFlight = _converterHelper.ToFlightEntity(model);
                await _flightRepository.CreateAsync(newFlight);
                ViewBag.Message = "Flight successfuly created and awaiting aproval";
            }

            model.StartAirports = _airportRepository.GetStartingAirports();
            model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
            model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
            return View(model);
        }


        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);
            if (flight == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEditViewModel(flight);

            model.StartAirports = _airportRepository.GetStartingAirports();
            model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
            model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.FlightEnd <= model.FlightStart)
                {
                    ModelState.AddModelError("FlightEnd", "You can't end a flight before it starts.");
                    model.StartAirports = _airportRepository.GetStartingAirports();
                    model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
                    model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
                    return View(model);
                }

                var flightForUpdate = await _flightRepository.GetByIdAsync(model.Id);
                if (flightForUpdate == null)
                {
                    return NotFound();
                }

                //update flight info
                flightForUpdate.StartAirportId = model.StartAirportId;
                flightForUpdate.EndAirportId = model.EndAirportId;
                flightForUpdate.FlightStart = model.FlightStart;
                flightForUpdate.FlightEnd = model.FlightEnd;
                flightForUpdate.FlightCompanyId = model.FlightCompanyId;
                flightForUpdate.BaseMilesPrice = model.BaseMilesPrice;
                
                
                await _flightRepository.UpdateAsync(flightForUpdate);
                ViewBag.Message = "Flight successfuly updated.";
            }

            model.StartAirports = _airportRepository.GetStartingAirports();
            model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
            model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);
            if (flight == null)
            {
                return NotFound();
            }
            flight.StartAirport = await _airportRepository.GetByIdAsync(flight.StartAirportId);
            flight.EndAirport = await _airportRepository.GetByIdAsync(flight.EndAirportId);
            flight.FlightCompany = await _partnerRepository.GetByIdAsync(flight.FlightCompanyId);
            if (flight.StartAirport == null || flight.EndAirport == null || flight.FlightCompany == null)
            {
                return NotFound();
            }



            var model = _converterHelper.ToDetailsViewModel(flight);
            return View(model);
        }

        public IActionResult ApprovalIndex()
        {
            var list = _flightRepository.GetAllWithAirportsAndPartners()
                    .Where(f => f.IsAproved == false)
                    .Select(f => new IndexViewModel
                    {
                        Id = f.Id,
                        StartAirportName = f.StartAirport.Name,
                        EndAirportName = f.EndAirport.Name,
                        FlightStart = f.FlightStart,
                        FlightCompanyName = f.FlightCompany.Name,
                        IsAproved = f.IsAproved
                    });
            return View(list);
        }



        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Aprove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);
            if (flight == null)
            {
                return this.NotFound();
            }

            flight.IsAproved = true;
            await _flightRepository.UpdateAsync(flight);

            return this.RedirectToAction(nameof(ApprovalIndex));
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var flight = await _flightRepository.GetByIdAsync(id.Value);
                if (flight == null)
                {
                    return NotFound();
                }

                await _flightRepository.DeleteAsync(flight);
                return StatusCode(200, "Success");

            }
            catch (Exception)
            {
                return StatusCode(520, "Unknown Error.");
            }
        }


        [HttpPost]
        public async Task<JsonResult> GetEndAirports(int? startAirportId)
        {
            var airports = await Task.Run(() => _airportRepository.GetEndAirports(startAirportId.Value));
            return this.Json(airports);
        }
    }
}