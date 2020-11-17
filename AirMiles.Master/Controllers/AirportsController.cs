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

        public AirportsController(
            IConverterHelper converterHelper,
            IAirportRepository airportRepository)
        {
            _converterHelper = converterHelper;
            _airportRepository = airportRepository;
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


        [Authorize(Roles = "SuperEmployee,Admin")]
        public IActionResult Requests()
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

            return this.RedirectToAction(nameof(Index));
        }
    }
}
