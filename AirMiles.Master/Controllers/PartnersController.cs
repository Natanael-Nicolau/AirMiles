using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Partners;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Controllers
{
    public class PartnersController : Controller
    {
        private readonly IConverterHelper _converterHelper;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IImageHelper _imageHelper;

        public PartnersController(
            IConverterHelper converterHelper,
            IPartnerRepository partnerRepository,
            IFlightRepository flightRepository,
            IImageHelper imageHelper)
        {
            _converterHelper = converterHelper;
            _partnerRepository = partnerRepository;
            _flightRepository = flightRepository;
            _imageHelper = imageHelper;
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
                //Defines the Photo as the default
                var path = $"~/images/Partners/no-logo.png";

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "Partners");
                }


                var entity = _converterHelper.ToPartnerEntity(model, path);
                await _partnerRepository.CreateAsync(entity);
                ViewBag.Message = "Partner successfuly created and awaiting aproval";
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

            var partner = await _partnerRepository.GetByIdAsync(id.Value);
            if (partner == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEditViewModel(partner);
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var partnerForUpdate = await _partnerRepository.GetByIdAsync(model.Id);

                //Initializes variable path with the old path
                var path = model.ImagePath;

                //Updates the path variable if the user selected a new image for upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "Partners");
                }


                //Updates the Entity
                partnerForUpdate.ImageUrl = path;
                partnerForUpdate.Name = model.Name;
                partnerForUpdate.Description = model.Description;
                partnerForUpdate.IsStarAlliance = model.IsStarAlliance;
                partnerForUpdate.WebsiteUrl = model.WebsiteUrl;

                
                await _partnerRepository.UpdateAsync(partnerForUpdate);
                ViewBag.Message = "Partner successfuly updated.";
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partner = await _partnerRepository.GetByIdAsync(id.Value);
            if (partner == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToDetailsViewModel(partner);
            return View(model);
        }

        [Authorize(Roles ="Admin,SuperEmployee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }           

            try
            {
                var partner = await _partnerRepository.GetByIdAsync(id.Value);
                if (partner == null)
                {
                    return NotFound();
                }

                var flight = _flightRepository.GetAll()
                    .Where(f => f.FlightCompanyId == partner.Id)
                    .FirstOrDefault();
                if (flight != null)
                {
                    return StatusCode(400, "Please delete all flights that use this company first!");
                }


                await _partnerRepository.DeleteAsync(partner);
                return StatusCode(200, "Success");

            }
            catch (Exception)
            {
                return StatusCode(520, "Unknown Error.");
            }
        }


        [Authorize(Roles = "SuperEmployee,Admin")]
        public IActionResult ApprovalIndex()
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

            return this.RedirectToAction(nameof(ApprovalIndex));
        }
    }
}