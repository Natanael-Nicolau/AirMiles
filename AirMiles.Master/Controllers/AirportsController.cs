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

        public IActionResult Index()
        {
            if (this.User.IsInRole("Employee"))
            {
                var list = _airportRepository.GetAll()
                    .Where(a => a.IsAproved && !a.IsDeleted)
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
            else if (this.User.IsInRole("SuperEmployee"))
            {
                var list = _airportRepository.GetAll()
                    .Where(a => !a.IsAproved && !a.IsDeleted)
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
            else
            {
                var list = _airportRepository.GetAll()
                    .Where(a => !a.IsDeleted)
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
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Aprove(int id)
        {
            var partner = await _airportRepository.GetByIdAsync(id);
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
