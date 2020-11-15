using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Partners;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirMiles.Master.Controllers
{
    public class PartnersController : Controller
    {
        private readonly IConverterHelper _converterHelper;
        private readonly IPartnerRepository _partnerRepository;

        public PartnersController(
            IConverterHelper converterHelper,
            IPartnerRepository partnerRepository)
        {
            _converterHelper = converterHelper;
            _partnerRepository = partnerRepository;
        }

        public IActionResult Index()
        {
            if (this.User.IsInRole("Employee"))
            {
                var list = _partnerRepository.GetAll()
                    .Where(p => p.IsAproved && !p.IsDeleted)
                    .Select(p => new IndexViewModel
                    {
                        Id = p.Id,
                        IsStarAlliance = p.IsStarAlliance,
                        CreationDate = p.CreationDate,
                        Name = p.Name,
                        IsAproved = p.IsAproved
                    });
                return View(list);
            }
            else if (this.User.IsInRole("SuperEmployee"))
            {
                var list = _partnerRepository.GetAll()
                    .Where(p => !p.IsAproved && !p.IsDeleted)
                    .Select(p => new IndexViewModel
                    {
                        Id = p.Id,
                        IsStarAlliance = p.IsStarAlliance,
                        CreationDate = p.CreationDate,
                        Name = p.Name,
                        IsAproved = p.IsAproved
                    });
                return View(list);
            }
            else
            {
                var list = _partnerRepository.GetAll()
                    .Where(p => !p.IsDeleted)
                    .Select(p => new IndexViewModel
                    {
                        Id = p.Id,
                        IsStarAlliance = p.IsStarAlliance,
                        CreationDate = p.CreationDate,
                        Name = p.Name,
                        IsAproved = p.IsAproved
                    });
                return View(list);
            }
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Aprove(int id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);
            if (partner == null)
            {
                return this.NotFound();
            }

            partner.IsAproved = true;
            await _partnerRepository.UpdateAsync(partner);

            return this.RedirectToAction(nameof(Index));
        }
    }
}
