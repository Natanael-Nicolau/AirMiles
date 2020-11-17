using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Partners;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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

        [Authorize(Roles = "Employee,Admin")]
        public IActionResult Index()
        {
            var list = _partnerRepository.GetAll()
                .Where(p => p.IsAproved)
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


        [Authorize(Roles = "SuperEmployee,Admin")]
        public IActionResult Requests()
        {
            var list = _partnerRepository.GetAll()
                    .Where(p => !p.IsAproved)
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


        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Aprove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partner = await _partnerRepository.GetByIdAsync(id.Value);
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