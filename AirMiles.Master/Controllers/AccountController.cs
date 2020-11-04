﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Data.Entities;
using AirMiles.Master.Data.Repositories;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AirMiles.Master.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(
            IUserRepository userRepository,
            IConfiguration configuration,
            IMailHelper mailHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var userList = _userRepository.GetIndexList();


            ICollection<IndexViewModel> modelList = new List<IndexViewModel>();
            foreach (var user in userList)
            {
                var model = _converterHelper.ToIndexViewModel(user);
                model.Position = await _userRepository.GetUserMainRoleAsync(user);

                modelList.Add(model);
            }

            return View(modelList.OrderBy(m => m.FullName));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                Roles = _userRepository.GetBackOfficeRoles()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            // Seed the Roles again
            model.Roles = _userRepository.GetBackOfficeRoles();

            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    string path;

                    if (model.Photo != null && model.Photo.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.Photo, "Users");
                    }
                    else
                    {
                        path = Path.Combine(Directory.GetCurrentDirectory(),
                            $"wwwroot\\images\\Users\\Default_User_Image.png");
                    }

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhotoUrl = path
                    };

                    var result = await _userRepository.AddUserAsync(user, "Password");

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created");
                        return View(model);
                    }

                    // Add user to Role
                    await _userRepository.AddUsertoRoleAsync(user, model.Role);

                    var myToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

                    var tokenLink = this.Url.Action("ConfirmAccount", "Account", new
                    {
                        userid = user.Id,
                        token = myToken,
                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Email, "Account Confirmation", $"<h1>Account Confirmation</h1>" +
                       $"To finish your account registration, " +
                       $"please click this link:</br></br><a href = \"{tokenLink}\">Confirm Account</a>");

                    this.ViewBag.Message = "The instructions to confirm your account have been sent to the email.";

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "A User with this email is alredy registered");
            }

            return View(model);
        }

        #region Login
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        //Direção de retorno
                        return this.Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }

            }

            this.ModelState.AddModelError(string.Empty, "Failed to login.");
            return this.View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutAsync();
            return this.RedirectToAction("Index", "Home");
        }
        #endregion

        #region PasswordManagement
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user.");
                    return this.View(model);
                    
                }

                var myToken = await _userRepository.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");
                this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                return this.View();

            }

            return this.View(model);
        }
        #endregion
    }
}
