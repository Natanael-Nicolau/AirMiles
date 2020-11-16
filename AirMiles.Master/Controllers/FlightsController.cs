﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Flights;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

            if (this.User.IsInRole("Employee"))
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
            else if (this.User.IsInRole("SuperEmployee"))
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
            else
            {
                var list = _flightRepository.GetAllWithAirportsAndPartners()
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
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                StartAirports = _airportRepository.GetStartingAirports(),
                EndAirports = _airportRepository.GetEndAirports(0),
                FlightCompanies = _partnerRepository.GetAllFlightCompanies()
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
                    ModelState.AddModelError("FlightEnd", "Invalid flight end date.");
                    model.StartAirports = _airportRepository.GetStartingAirports();
                    model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
                    model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
                    return View(model);
                }

                var newFlight = _converterHelper.ToFlightEntity(model);
                await _flightRepository.CreateAsync(newFlight);
                model.StartAirports = _airportRepository.GetStartingAirports();
                model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
                model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
                ViewBag.Message = "Flight successfuly created and awaiting aproval";
            }


            model.StartAirports = _airportRepository.GetStartingAirports();
            model.EndAirports = _airportRepository.GetEndAirports(model.StartAirportId);
            model.FlightCompanies = _partnerRepository.GetAllFlightCompanies();
            return View(model);
        }


        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Aprove(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
            {
                return this.NotFound();
            }

            flight.IsAproved = true;
            await _flightRepository.UpdateAsync(flight);

            return this.RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public JsonResult GetEndAirports(int startAirportId)
        {
            var airports = _airportRepository.GetEndAirports(startAirportId);
            return this.Json(airports.OrderBy(a => a.Text));
        }
    }
}