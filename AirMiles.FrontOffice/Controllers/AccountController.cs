using AirMiles.FrontOffice.Helpers;
using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.FrontOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMileRepository _mileRepository;

        public AccountController(IUserRepository userRepository, IMailHelper mailHelper, IClientRepository clientRepository, IConverterHelper converterHelper, ITransactionRepository transactionRepository, IMileRepository mileRepository)
        {
            _userRepository = userRepository;
            _mailHelper = mailHelper;
            _clientRepository = clientRepository;
            _converterHelper = converterHelper;
            _transactionRepository = transactionRepository;
            _mileRepository = mileRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Login

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Edit", "Account");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _clientRepository.GetByIdAsync(Convert.ToInt32(model.ClientId));

                if (client != null)
                {
                    var user = await _userRepository.GetUserByIdAsync(client.UserId);

                    if (user != null)
                    {
                        var result = await _userRepository.LoginAsync(user.UserName, model.Password, model.RememberMe);

                        if (result.Succeeded)
                        {
                            return this.RedirectToAction("Edit", "Account");
                        }

                        this.ModelState.AddModelError(string.Empty, "Failed to login.");

                        return View(model);
                    }
                }

                this.ModelState.AddModelError(string.Empty, "This account does not exist");

                return View(model);
            }
            this.ModelState.AddModelError(string.Empty, "Failed to login.");

            return View(model);
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutAsync();

            //TODO Change this
            return this.RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterNewClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Gets the User
                var user = await _userRepository.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    // Creates a new Client
                    var client = new Client
                    {

                        User = new User
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = model.Username,
                            Email = model.Username,
                            PhotoUrl = "~/images/Users/Default_User_Image.png"
                        }
                    };

                    // Adds the User to the DataBase
                    var result = await _userRepository.AddUserAsync(user, model.Password);

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created");
                        return View(model);
                    }

                    // Add user to Role
                    await _userRepository.AddUsertoRoleAsync(user, "Client");

                    // Adds the Client to the DataBase
                    await _clientRepository.CreateAsync(client);

                    // Retrieves the Client ID
                    var clientID = await _clientRepository.GetByEmailAsync(model.Username);

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
                        _mailHelper.SendMail(model.Username, "Account Confirmation", $"<h1>Account Confirmation</h1>" +
                       $"To finish your account registration, " +
                       $"please click this link:</br></br><a href = \"{tokenLink}\">Confirm Account</a>"
                       + $"</br></br>Your Account ID is : {clientID:D9}");
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

            var result = await _userRepository.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return this.NotFound();
            }

            return View();
        }

        #endregion

        #region PasswordManagement

        public IActionResult ForgotPassword()
        {
            ViewData["Message"] = "Insert the registered email to recover your account.";
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
                ViewData["Message"] = "The instructions to recover your password have been sent!";
                return this.View();

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
                        //return this.RedirectToAction(nameof(Details));
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

        #region CRUD

        [Authorize]
        public async Task<IActionResult> Edit()
        {

            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            if (client == null)
            {
                return NotFound();
            }

            var user = await _userRepository.GetUserByIdAsync(client.UserId);

            if(user == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEditViewModel(client, user);

            if(this.User.IsInRole("Gold"))
            {
                model.BackgroundPath = "/lib/ClientTemplate/img/status/Gold.jpg";
            }
            else if (this.User.IsInRole("Silver"))
            {
                model.BackgroundPath = "/lib/ClientTemplate/img/status/Silver.jpg";
            }
            else if(this.User.IsInRole("Basic"))
            {
                model.BackgroundPath = "/lib/ClientTemplate/img/status/Basic.jpg";
            }
            return View(model);
        }

        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(EditViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!(this.User.IsInRole("Admin") || this.User.Identity.Name == model.Email))
        //        {
        //            return this.Unauthorized();
        //        }

        //        var user = await _userRepository.GetUserByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            return this.NotFound();
        //        }

        //        //initializes a new Path
        //        string path;
        //        if (model.Photo != null && model.Photo.Length > 0)
        //        {
        //            path = await _imageHelper.UploadImageAsync(model.Photo, "Users");
        //        }
        //        else
        //        {
        //            //Defines the Photo as the default
        //            path = model.PhotoUrl;
        //        }

        //        //Updates the user roles
        //        if (!(await _userRepository.IsUserInRoleAsync(user, model.Role)))
        //        {
        //            var currentRole = await _userRepository.GetUserMainRoleAsync(user);
        //            var roleResult = await _userRepository.RemoveFromRole(user, currentRole);
        //            if (!roleResult.Succeeded)
        //            {
        //                ModelState.AddModelError(string.Empty, "Failed to update user Role");
        //                model.Roles = _userRepository.GetBackOfficeRoles();
        //                return View(model);
        //            }

        //            await _userRepository.AddUsertoRoleAsync(user, model.Role);
        //        }

        //        //Updates the user entity
        //        user.FirstName = model.FirstName;
        //        user.LastName = model.LastName;
        //        user.BirthDate = model.BirthDate;
        //        user.PhotoUrl = path;

        //        var result = await _userRepository.UpdateUserAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            ModelState.AddModelError(string.Empty, "Failed to update User");
        //            model.Roles = _userRepository.GetBackOfficeRoles();
        //            return View(model);
        //        }


        //        return RedirectToAction(nameof(Details), new { email = model.Email });

        //    }

        //    model.Roles = _userRepository.GetBackOfficeRoles();
        //    return View(model);
        //}

        #endregion

        #region Miles&Go

        public async Task<IActionResult> BalanceMovements()
        {
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            if(client == null)
            {
                return NotFound();
            }

            var transactions = _transactionRepository.GetAll().Where(t => t.ClientID == client.Id).ToList();

            var model = _converterHelper.ToBalanceMovementsViewModel(transactions);

            return View(model);
        }

        #endregion

        #region MilesStore

        public IActionResult BuyMiles()
        {
            var models = new List<BuyMilesViewModel>();

            var i = 1;

            while(i < 6)
            {
               var mile =  new BuyMilesViewModel
                {
                    Amount = 2000 * i,
                    Price = 70 * i,
                    Selected = false
                };

                i++;

                models.Add(mile);
            }

            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> BuyMiles(IList<BuyMilesViewModel> models)
        {
            if (ModelState.IsValid)
            {
                var model = models.Where(m => m.Selected).FirstOrDefault();

                if(model == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Please select an amount");
                    return View(models);
                }

                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                var boughtMiles = model.Amount + client.BoughtMiles;

                if(boughtMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only buy a maximum of 20.000 Miles per Year");

                    return View(models);
                }

                var mile = _converterHelper.ToMile(model, client.Id);

                var transaction =  _converterHelper.ToTransaction(model, mile);

                try
                {
                    await _mileRepository.CreateAsync(mile);

                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                try
                {
                    await _transactionRepository.CreateAsync(transaction);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                try
                {
                    client.BoughtMiles = boughtMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                ViewBag.Message = "Your purchase was successful!";
            }
            return View(models);
        }

        public IActionResult TransferMiles()
        {
            var model = new TransferMilesViewModel
            {
                Amount = 2000,
                GiftedClientID = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TransferMiles(TransferMilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Gets the current Client
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                // Gets the client to gift Miles
                var giftedClient = await _clientRepository.GetByIdAsync(Convert.ToInt32(model.GiftedClientID));

                // Checks if the giftedClient exists
                if(giftedClient == null)
                {
                    this.ModelState.AddModelError(string.Empty, "The selected Client does not exist. Please try again!");
                    return View(model);
                }

                //Checks if the maximum value of transfered miles has been reached
                var transferMiles = model.Amount + client.TransferedMiles;

                if (transferMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only transfer a maximum of 20.000 Miles per Year");

                    return View(model);
                }

                var clientMiles = _mileRepository.GetAll().Where(m => m.ClientId == client.Id && !m.IsDeleted && m.MilesTypeId == 2).OrderBy(m => m.ExpirationDate).ToList();

                var clientTotalMiles = 0;

                foreach (var x in clientMiles)
                {
                    clientTotalMiles += x.Qtd;
                }

                if(clientTotalMiles < model.Amount)
                {
                    this.ModelState.AddModelError(string.Empty, "You dont have the necessary Miles to perform this transfer. Please try again with a lower value!");

                    return View(model);
                }

                // Creates a new Mile Object for the giftedClient
                var mile = new Mile
                {
                    ClientId = giftedClient.Id,
                    Qtd = model.Amount,
                    MilesTypeId = 2,
                    ExpirationDate = DateTime.Now.AddYears(3),
                    IsAproved = true,
                    IsDeleted = false,
                };

                // Creates the Transaction on the giftedClient end
                var receivedMiles = new Transaction
                {
                    Description = "Transfered",
                    Value = mile.Qtd,
                    TransactionDate = DateTime.Now,
                    Price = 0,
                    ClientID = giftedClient.Id,
                    IsAproved = true,
                    IsCreditCard = false
                };

                // Creates the Transaction on the Client that will transfer miles end
                var giftedMiles = new Transaction
                {
                    Description = "Transfered",
                    Value = - mile.Qtd,
                    TransactionDate = DateTime.Now,
                    Price = 0,
                    ClientID = client.Id,
                    IsAproved = true,
                    IsCreditCard = false
                };

                // 
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                try
                {
                    await _transactionRepository.CreateAsync(receivedMiles);
                    await _transactionRepository.CreateAsync(giftedMiles);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }
                try
                {
                    for(int i = transferMiles, j = 0; i != 0; j++)
                    {
                        if(i < 0)
                        {
                            throw new Exception();
                        }

                        var currentMile = clientMiles[j];

                        if(currentMile.Qtd >= i)
                        {
                            i = 0;
                            currentMile.Qtd -= i;
                        }
                        else
                        {
                            i -= currentMile.Qtd;
                            currentMile.Qtd = 0;
                        }

                        if(currentMile.Qtd == 0)
                        {
                            await _mileRepository.DeleteAsync(currentMile);
                        }
                        else
                        {
                            await _mileRepository.UpdateAsync(currentMile);
                        }
                           
                    }
                }
                catch (Exception)
                {

                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                try
                {
                    client.TransferedMiles = transferMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                ViewBag.Message = "Your transfer was successfull!";
                return View();
            }
            
            this.ModelState.AddModelError(string.Empty, "Please select a valid Client Id");
            return View(model);
        }

        #endregion
    }
}