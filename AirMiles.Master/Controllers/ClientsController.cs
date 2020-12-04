using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Clients;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirMiles.Master.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMileRepository _mileRepository;
        private readonly IConverterHelper _converterHelper;

        public ClientsController(
            IClientRepository clientRepository,
            IUserRepository userRepository,
            IMileRepository mileRepository,
            IConverterHelper converterHelper)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _mileRepository = mileRepository;
            _converterHelper = converterHelper;
        }


        [Authorize(Roles ="Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            var clients = _clientRepository.GetAllWithUsers()
                .Select(c => new IndexViewModel
            {
                    Id = c.Id,
                    Email = c.User.Email,
                    FullName = c.User.FullName
            }).ToList();

            for (int i = 0; i < clients.Count(); i++)
            {
                clients[i].Status = await _userRepository.GetClientStatusRoleAsync(clients[i].Email);
            }


            return View(clients);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }
            client.User = await _userRepository.GetUserByIdAsync(client.UserId);
            if (client.User == null)
            {
                return NotFound();
            }
            var status = await _userRepository.GetClientStatusRoleAsync(client.User.Email);
            if (status == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToDetailsViewModel(client, status);

            
            model.TotalStatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id);
            model.TotalBonusMiles = _mileRepository.GetClientTotalBonusMiles(client.Id);

            return View(model);
        }

        [Authorize(Roles = "Admin,SuperEmployee")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var client = await _clientRepository.GetByIdAsync(id.Value);
                if (client == null)
                {
                    return NotFound();
                }

                await _clientRepository.DeleteAsync(client);
                return StatusCode(200, "Success");

            }
            catch (Exception)
            {
                return StatusCode(520, "Unknown Error.");
            }
        }
    }
}
