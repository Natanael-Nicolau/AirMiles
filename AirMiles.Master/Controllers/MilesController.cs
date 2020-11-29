using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Miles;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirMiles.Master.Controllers
{
    public class MilesController : Controller
    {
        private readonly IMilesRequestRepository _milesRequestRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMileRepository _mileRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConverterHelper _converterHelper;

        public MilesController(
            IMilesRequestRepository milesRequestRepository,
            IClientRepository clientRepository,
            IMileRepository mileRepository,
            ITransactionRepository transactionRepository,
            IPartnerRepository partnerRepository,
            IUserRepository userRepository,
            IConverterHelper converterHelper)
        {
            _milesRequestRepository = milesRequestRepository;
            _clientRepository = clientRepository;
            _mileRepository = mileRepository;
            _transactionRepository = transactionRepository;
            _partnerRepository = partnerRepository;
            _userRepository = userRepository;
            _converterHelper = converterHelper;
        }

        public IConverterHelper ConverterHelper { get; }


        #region BackOffice

        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Requests()
        {
            var list = _milesRequestRepository.GetAll().Where(r => !r.IsAproved).ToList();

            var model = new List<RequestsIndexViewModel>();
            foreach (var request in list)
            {
                request.Client = await _clientRepository.GetByIdAsync(request.ClientId);
                request.Client.User = await _userRepository.GetUserByIdAsync(request.Client.UserId);
                request.Partner = await _partnerRepository.GetByIdAsync(request.PartnerId);
                model.Add(_converterHelper.ToRequestsIndexViewModel(request));
            }

            return View(model);
        }

        public async Task<IActionResult> Aprove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _milesRequestRepository.GetByIdAsync(id.Value);
            if (request == null)
            {
                return NotFound();
            }
            request.IsAproved = true;
            await _milesRequestRepository.UpdateAsync(request);

            await _transactionRepository.CreateAsync(new Transaction {
                ClientID = request.ClientId,
                Description = "Request Aproval",
                IsAproved = true,
                IsCreditCard = false,
                Price = 0,
                TransactionDate = DateTime.Now,
                IsDeleted = false,
                Value = request.MilesAmount
            });

            await _mileRepository.CreateAsync(new Mile {
                ClientId = request.ClientId,
                ExpirationDate = DateTime.Now.AddYears(3),

                IsAproved = true,
                MilesTypeId = 2,
                Qtd = request.MilesAmount,
                IsDeleted = false
            });
            var partner = await _partnerRepository.GetByIdAsync(request.PartnerId);
            if (partner.IsStarAlliance)
            {
                await _mileRepository.CreateAsync(new Mile
                {
                    ClientId = request.ClientId,
                    ExpirationDate = DateTime.Now.AddYears(3),
                    IsAproved = true,
                    MilesTypeId = 1,
                    Qtd = request.MilesAmount,
                    IsDeleted = false
                });
            }

            return RedirectToAction(nameof(Requests));
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var request = await _milesRequestRepository.GetByIdAsync(id.Value);
                if (request == null)
                {
                    return NotFound();
                }

                await _milesRequestRepository.DeleteAsync(request);
                return StatusCode(200, "Success");

            }
            catch (Exception)
            {
                return StatusCode(520, "Unknown Error.");
            }
        }

        #endregion

    }
}
