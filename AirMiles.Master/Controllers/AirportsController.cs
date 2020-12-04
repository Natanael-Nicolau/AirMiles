using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Airports;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirMiles.Master.Controllers
{
    public class AirportsController : Controller
    {
        private readonly IConverterHelper _converterHelper;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightRepository _flightRepository;

        public AirportsController(
            IConverterHelper converterHelper,
            IAirportRepository airportRepository,
            IFlightRepository flightRepository)
        {
            _converterHelper = converterHelper;
            _airportRepository = airportRepository;
            _flightRepository = flightRepository;
        }

        

        [Authorize(Roles = "Employee,Admin")]
        public IActionResult Index()
        {
            var list = _airportRepository.GetAll()
                    .Where(a => a.IsAproved)
                    .Select(a => new IndexViewModel
                    {
                        Id = a.Id,
                        IATA = a.IATA,
                        FullLocation = a.FullLocation,
                        Name = a.Name,
                        IsAproved = a.IsAproved
                    });

            return View(list);

        }



        [Authorize(Roles = "Employee,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var airport = await _airportRepository.GetByIataAsync(model.IATA);
                if (airport != null)
                {
                    ModelState.AddModelError(nameof(model.IATA), "There's already an Airport registered with the given IATA.");
                    return View(model);
                }

                var entity = _converterHelper.ToAirportEntity(model);
                await _airportRepository.CreateAsync(entity);
                ViewBag.Message = "Airport successfuly created and awaiting aproval.";
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);
            if (airport == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEditViewModel(airport);

            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var airportForUpdate = await _airportRepository.GetByIdAsync(model.Id);
                if (airportForUpdate == null)
                {
                    return NotFound();
                }

                //Checks if there was a change in the IATA that could be a repeat of another airport
                if (airportForUpdate.IATA != model.IATA)
                {
                    var airport = await _airportRepository.GetByIataAsync(model.IATA);
                    if (airport != null)
                    {
                        ModelState.AddModelError(nameof(model.IATA), "There's already an Airport registered with the given IATA.");
                        return View(model);
                    }
                }

                //Update Entity
                airportForUpdate.IATA = model.IATA;
                airportForUpdate.Latitude = model.Latitude;
                airportForUpdate.Longitude = model.Longitude;
                airportForUpdate.Country = model.Country;
                airportForUpdate.City = model.City;
                airportForUpdate.Name = model.Name;
                airportForUpdate.Region = model.Region;

                await _airportRepository.UpdateAsync(airportForUpdate);
                ViewBag.Message = "Airport successfuly updated!";
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _airportRepository.GetByIdAsync(id.Value);
            if (airport == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToDetailsViewModel(airport);

            return View(model);
        }


        [Authorize(Roles = "SuperEmployee,Admin")]
        public IActionResult ApprovalIndex()
        {
            var list = _airportRepository.GetAll()
                    .Where(a => !a.IsAproved)
                    .Select(a => new IndexViewModel
                    {
                        Id = a.Id,
                        IATA = a.IATA,
                        FullLocation = a.FullLocation,
                        Name = a.Name,
                        IsAproved = a.IsAproved
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

            var partner = await _airportRepository.GetByIdAsync(id.Value);
            if (partner == null)
            {
                return this.NotFound();
            }

            partner.IsAproved = true;
            await _airportRepository.UpdateAsync(partner);

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
                var airport = await _airportRepository.GetByIdAsync(id.Value);
                if (airport == null)
                {
                    return NotFound();
                }

                var flight = _flightRepository.GetAll()
                    .Where(f => f.StartAirportId == airport.Id || f.EndAirportId == airport.Id)
                    .FirstOrDefault();
                if (flight != null)
                {
                    return StatusCode(400, "Please delete all flights that go through this airport first!");
                }


                await _airportRepository.DeleteAsync(airport);
                return StatusCode(200, "Success");

            }
            catch (Exception)
            {
                return StatusCode(520, "Unknown Error.");
            }
        }
    }
}
