using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirMiles.Master.Helpers;
using AirMiles.Master.Models.Account;
using AIrMiles.WebApp.Common.Data.Repositories;
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
                        path = $"~/images/Users/Default_User_Image.png";
                    }

                    //Initializes a new User
                    user = _converterHelper.ToUserEntity(model, path);

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
                    try
                    {
                        _mailHelper.SendMail(model.Email, "Account Confirmation", $"<h1>Account Confirmation</h1>" +
                       $"To finish your account registration, " +
                       $"please click this link:</br></br><a href = \"{tokenLink}\">Confirm Account</a>");
                        this.ViewBag.Message = "The instructions to confirm your account have been sent to the email.";
                    }
                    catch (Exception)
                    {
                        this.ModelState.AddModelError(string.Empty, "Error sending the email, please try again in a few minutes");
                    }
                    return this.View(model);
                }

                ModelState.AddModelError(string.Empty, "A User with this email is already registered");
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string email)
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
            var model = _converterHelper.ToEditViewModel(user, role);
            model.Roles = _userRepository.GetBackOfficeRoles();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!(this.User.IsInRole("Admin") || this.User.Identity.Name == model.Email))
                {
                    return this.Unauthorized();
                }

                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return this.NotFound();
                }

                //initializes a new Path
                string path;
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.Photo, "Users");
                }
                else
                {
                    //Defines the Photo as the default
                    path = model.PhotoUrl;
                }

                //Updates the user roles
                if (!(await _userRepository.IsUserInRoleAsync(user, model.Role)))
                {
                    var currentRole = await _userRepository.GetUserMainRoleAsync(user);
                    var roleResult = await _userRepository.RemoveFromRole(user, currentRole);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update user Role");
                        model.Roles = _userRepository.GetBackOfficeRoles();
                        return View(model);
                    }

                    await _userRepository.AddUsertoRoleAsync(user, model.Role);                    
                }

                //Updates the user entity
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.BirthDate = model.BirthDate;
                user.PhotoUrl = path;

                var result = await _userRepository.UpdateUserAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update User");
                    model.Roles = _userRepository.GetBackOfficeRoles();
                    return View(model);
                }


                return RedirectToAction(nameof(Details), new { email = model.Email });

            }

            model.Roles = _userRepository.GetBackOfficeRoles();
            return View(model);
        }
        #endregion





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
                var result = await _userRepository.LoginAsync(model.Username, model.Password, model.RememberMe);
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
        [ValidateAntiForgeryToken]
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

                    // Changes the default password to the user chosen password"
                    result = await _userRepository.ChangePasswordAsync(user, "Password", model.Password);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("Login");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, "Error while changing the password.");
                        return View(model);
                    }


                }

                this.ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            return View(model);
        }



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
                    new { 
                        token = myToken,
                        userId = user.Id
                    }, protocol: HttpContext.Request.Scheme);

                try
                {
                    _mailHelper.SendMail(model.Email, "AirMiles Password Reset", $"<h1>AirMiles Password Reset</h1>" +
                            $"To reset the password click in this link:</br></br>" +
                            $"<a href = \"{link}\">Reset Password</a>");
                    this.ViewBag.Message = "The instructions to recover your password have been sent to email.";
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "Error sending the email, please try again in a few minutes");
                }
                return this.View(model);

            }

            return this.View(model);
        }


        public async Task<IActionResult> ResetPassword(string userId, string token)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    // Confirms the Email
                    var result = await _userRepository.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("Login");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, "Error while changing the password.");
                        return View(model);
                    }
                }
                this.ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            return View(model);
        }


        public IActionResult ChangePassword()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userRepository.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction(nameof(Details));
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(model);
        }
        #endregion
    }
}
