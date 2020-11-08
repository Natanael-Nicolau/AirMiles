using System;
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




        #region CRUD
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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            // Seed the Roles again
            model.Roles = _userRepository.GetBackOfficeRoles();

            if (ModelState.IsValid)
            {
                //Gets the user
                var user = await _userRepository.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    // Initializes an empty path
                    string path;

                    if (model.Photo != null && model.Photo.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.Photo, "Users");
                    }
                    else
                    {
                        //Defines the Photo as the default
                        path = Path.Combine(Directory.GetCurrentDirectory(),
                            $"wwwroot\\images\\Users\\Default_User_Image.png");
                    }

                    //Initializes a new User
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Username,
                        Email = model.Email,
                        PhotoUrl = path
                    };

                    // Adds the User to the DataBase
                    var result = await _userRepository.AddUserAsync(user, "Password");

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created");
                        return View(model);
                    }

                    // Add user to Role
                    await _userRepository.AddUsertoRoleAsync(user, model.Role);

                    // Creates a Token in order to confirm the email
                    var myToken = await _userRepository.GenerateEmailConfirmationTokenAsync(user);

                    // Defines the Link with its properties to be sent in the email
                    var tokenLink = this.Url.Action("ConfirmAccount", "Account", new
                    {
                        userid = user.Id,
                        token = myToken,
                    }, protocol: HttpContext.Request.Scheme);

                    //Sends an Email to the User with the TokenLink
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

        [Authorize]
        public async Task<IActionResult> Details(string email)
        {
            if (!(this.User.IsInRole("Admin") || this.User.Identity.Name == email))
            {
                return this.Unauthorized();
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return this.NotFound();
            }
            var role = await _userRepository.GetUserMainRoleAsync(user);


            var model = _converterHelper.ToDetailsViewModel(user, role);
            return View(model);
        }
        #endregion




        public async Task<IActionResult> ConfirmAccount(string userId, string token)
        {
            // Verifies if the userId and token are not empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return this.NotFound();
            }

            // Gets the User through its Id
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            // Initializes a new Model, filling the fields 
            var model = new ResetPasswordViewModel
            {
                UserName = user.UserName,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAccount(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.UserName);

                if (user != null)
                {
                    // Confirms the Email
                    var result = await _userRepository.ConfirmEmailAsync(user, model.Token);

                    if (!result.Succeeded)
                    {
                        return this.NotFound();
                    }

                    result = await _userRepository.ChangePasswordAsync(user, "Password", model.Password);

                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("Login");
                    }
                    else
                    {
                        this.ViewBag.Message = "Error while changing the password.";
                    }

                    return View(model);
                }

                this.ViewBag.Message = "User not found.";

                return View(model);
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
