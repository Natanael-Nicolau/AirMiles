using AirMiles.FrontOffice.Helpers;
using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

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
        private readonly IImageHelper _imageHelper;

        public AccountController(IUserRepository userRepository, IMailHelper mailHelper, IClientRepository clientRepository, IConverterHelper converterHelper, ITransactionRepository transactionRepository, IMileRepository mileRepository, IImageHelper imageHelper)
        {
            _userRepository = userRepository;
            _mailHelper = mailHelper;
            _clientRepository = clientRepository;
            _converterHelper = converterHelper;
            _transactionRepository = transactionRepository;
            _mileRepository = mileRepository;
            _imageHelper = imageHelper;
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
                        RevisionMonth = DateTime.Now.Month,
                        IsAproved = true,
                        IsDeleted = false,
                        User = new User
                        {
                            BirthDate = new DateTime(1990, 2, 15),
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = model.Username,
                            Email = model.Username,
                            PhotoUrl = "~/images/Users/Default_User_Image.png"
                        }
                    };

                    // Adds the User to the DataBase
                    var result = await _userRepository.AddUserAsync(client.User, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created");
                        return View(model);
                    }

                    // Add user to Role
                    await _userRepository.AddUsertoRoleAsync(client.User, "Client");
                    await _userRepository.AddUsertoRoleAsync(client.User, "Basic");
                    // Adds the Client to the DataBase
                    await _clientRepository.CreateAsync(client);

                    // Retrieves the Client ID
                    var clientID = await _clientRepository.GetByEmailAsync(model.Username);

                    // Creates a Token in order to confirm the email
                    
                    var myToken = await _userRepository.GenerateEmailConfirmationTokenAsync(client.User);

                    // Defines the Link with its properties to be sent in the email
                    var tokenLink = this.Url.Action("ConfirmAccount", "Account", new
                    {
                        userid = client.User.Id,
                        token = myToken,
                    }, protocol: HttpContext.Request.Scheme);

                    //Sends an Email to the User with the TokenLink
                    try
                    {
                        _mailHelper.SendMail(model.Username, "Account Confirmation", $"<h1>Account Confirmation</h1>" +
                       $"To finish your account registration, " +
                       $"please click this link: <a href = \"{tokenLink}\">Confirm Account</a>"
                       + $"<br/><br/>Your Account ID is: {clientID.Id:D9}");
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
                // Gets the user
                var user = await _userRepository.GetUserByEmailAsync(model.Email);

                // Reports back an error if the user does not exist
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user.");
                    return this.View(model);

                }

                // Generates a token
                var myToken = await _userRepository.GeneratePasswordResetTokenAsync(user);

                // Builds a link which has the generated token
                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                // Sends an email with a custom message to the client email with the instruction to recover his account
                _mailHelper.SendMail(model.Email, "Cinel Air Miles Password Reset", $"<h1>Password Reset</h1>" +
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
            // Gets the authenticated client
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            // Extra Security
            if (client == null)
            {
                return NotFound();
            }

            // Gets the object user that belongs to the authenticated client
            var user = await _userRepository.GetUserByIdAsync(client.UserId);

            // Extra Security
            if (user == null)
            {
                return NotFound();
            }

            // Converts to an EditViewModel, with the parameters being the objects client and user
            var model = _converterHelper.ToEditViewModel(client, user, AssignEditModelBackgroundPath());

            model.StatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id).ToString();
            model.BonusMiles = _mileRepository.GetClientTotalBonusMiles(client.Id).ToString();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile photo, int clientID)
        {
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            var user = await _userRepository.GetUserByIdAsync(client.UserId);

            var beforeModel = _converterHelper.ToEditViewModel(client, user, AssignEditModelBackgroundPath());

            beforeModel.StatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id).ToString();
            beforeModel.BonusMiles = _mileRepository.GetClientTotalBonusMiles(client.Id).ToString();

            if (photo != null && photo.Length > 0)
            {
                string path = await _imageHelper.UploadImageAsync(photo, "Users", clientID);
                user.PhotoUrl = path;
            }
            else
            {
                return View(beforeModel);
            }

            // Photo was updated correctly
            var updatedModel = _converterHelper.ToEditViewModel(client, user, AssignEditModelBackgroundPath());

            try
            {
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception)
            {
                this.ModelState.AddModelError(string.Empty, "An error occured while updating your information.");

                return View(beforeModel);
            }

            updatedModel.StatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id).ToString();
            updatedModel.BonusMiles = _mileRepository.GetClientTotalBonusMiles(client.Id).ToString();

            return View(updatedModel);
        }

        [HttpPost]
        public IActionResult EditInfo(string name, string email, string birthDate)
        {
            return StatusCode(422,"Email");
        }

        #endregion

        #region MilesCard

        public async Task<IActionResult> MilesCard(MilesCardViewModel model)
        {
            // Gets the authenticated client
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            // Extra Security
            if (client == null)
            {
                return NotFound();
            }

            // Gets the object user that belongs to the authenticated client
            var user = await _userRepository.GetUserByIdAsync(client.UserId);

            // Extra Security
            if (user == null)
            {
                return NotFound();
            }

            model = _converterHelper.ToMilesCardViewModel(client, user);

            if (User.IsInRole("Basic"))
            {
                model.Status = "Basic";
                model.BackColor = "#00c292";
                model.StatusPhoto = "/lib/ClientTemplate/img/card/plane_basic.png";
            }
            else if (User.IsInRole("Silver"))
            {
                model.Status = "Silver";
                model.BackColor = "silver";
                model.StatusPhoto = "/lib/ClientTemplate/img/card/plane_silver.png";
            }
            else if (User.IsInRole("Gold"))
            {
                model.Status = "Gold";
                model.BackColor = "#FFDF01";
                model.StatusPhoto = "/lib/ClientTemplate/img/card/plane_gold.png";
            }

            model.StatusMiles = _mileRepository.GetClientTotalStatusMiles(client.Id).ToString();
            model.BonusMiles = _mileRepository.GetClientTotalBonusMiles(client.Id).ToString();

            return View(model);
        }

        #endregion

        #region Miles&Go

        public async Task<IActionResult> BalanceMovements()
        {
            var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

            if (client == null)
            {
                return NotFound();
            }

            var transactions = _transactionRepository.GetAllByClientId(client.Id).OrderByDescending(t => t.TransactionDate).ToList();


            var model = _converterHelper.ToBalanceMovementsViewModel(transactions);

            return View(model);
        }

        public IActionResult DownloadBalanceMovementsPDF()
        {
            var data = Request.Query["info"].ToString();

            var lines = data.Split(",");

            var headers = Request.Query["headers"].ToString();

            var columns = headers.Split(",");

            var rows = lines.Length / columns.Length;

            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(ms))
                {
                    writer.SetCloseStream(false);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    Table table = new Table(new float[10]).UseAllAvailableWidth();


                    foreach (var value in columns)
                    {
                        Cell header = new Cell().Add(new Paragraph(value));
                        header.SetTextAlignment(TextAlignment.CENTER);
                        header.SetBackgroundColor(new DeviceRgb(0, 194, 146));
                        table.AddHeaderCell(header);
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        Cell cell = new Cell().Add(new Paragraph(lines[i]));
                        cell.SetTextAlignment(TextAlignment.CENTER);
                        table.AddCell(cell);

                        if ((i+1) % columns.Length == 0 && i != 0)
                        {
                            table.StartNewRow();
                        }
                    }

                    document.Add(table);
                    document.Close();

                    byte[] file = ms.ToArray();
                    ms.Write(file, 0, file.Length);
                    ms.Position = 0;

                    return File(file, "application/pdf", "Balance&Movements.pdf");
                }
            }
        }

        #endregion

        #region MilesStore

        public IActionResult BuyMiles()
        {
            // Generates a new instance of BuyMilesViewModel
            var models = new List<BuyMilesViewModel>();

            // Creates a new integer to use in the while 
            var i = 1;

            // Assigns values to the new BuyMilesViewModel
            while (i < 11)
            {
                var mile = new BuyMilesViewModel
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
                // Selects the value selected by the client
                var model = models.Where(m => m.Selected).FirstOrDefault();

                // Returns an error in case no value was selected
                if (model == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Please select an amount");
                    return View(models);
                }

                // Gets the client by Email
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                // Adds the value of the selected value amount with the pre-existing client Bought Miles and assigns it to a variable
                var boughtMiles = model.Amount + client.BoughtMiles;

                // Checks if the value is bigger than 20000 and if true, returns an error
                if (boughtMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only buy a maximum of 20.000 Miles per Year");

                    return View(models);
                }


                // Converts the selected value to a Mile Object
                var mile = _converterHelper.ToMile(model, client.Id, 1);

                // Converts the selected value and the new Mile Object to a Transaction Object 
                var transaction = _converterHelper.ToTransaction(model, mile);

                // Creates a new Mile in the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);

                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                // Creates a new Transaction in the DataBase
                try
                {
                    await _transactionRepository.CreateAsync(transaction);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(models);
                }

                // Updates the client
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
                // Returns a message if the operation was valid
                ViewBag.Message = "Your purchase was successful!";
            }
            return View(models);
        }

        public IActionResult TransferMiles()
        {
            // Generates a new instance of TransferMilesViewModel
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
                if (giftedClient == null)
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

                // Gets a list of the client Bonus Miles
                var clientMiles = _mileRepository.GetAllBonusMiles(client.Id);

                // Creates a new var to hold the client Total Bonus Miles
                var clientTotalMiles = _mileRepository.GetClientTotalBonusMiles(client.Id);

                // Checks if the client has enough Miles to perform the Transfer Operation
                if (clientTotalMiles < model.Amount)
                {
                    this.ModelState.AddModelError(string.Empty, "You dont have the necessary Miles to perform this transfer. Please try again with a lower value!");

                    return View(model);
                }

                // Creates a new Mile Object for the giftedClient
                var mile = _converterHelper.ToMile(model.Amount, giftedClient.Id, 1);
                    
                // Creates the Transaction on the giftedClient end
                var receivedMiles = _converterHelper.ToTransaction(giftedClient.Id, mile.Qtd);

                // Creates the Transaction on the Client that will transfer miles end
                // IMPORTANT NOTE:
                // SUBTRACT THE MILES QUANTITY
                var giftedMiles = _converterHelper.ToTransaction(giftedClient.Id, -mile.Qtd);

                // Creates a new Mile on the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your transfer.Please try again later");
                    return View(model);
                }

                // Creates two transaction on the Database
                // One for the gifter and another for the receiver
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

                // Transfers the Miles
                var transferSuccess = await _mileRepository.SpendMilesAsync(clientMiles, transferMiles);

                if(!transferSuccess)
                {

                    this.ModelState.AddModelError(string.Empty, "There was a critical error with the transfer algorithm. Please contact the Administrator as soon as possible");
                    return View(model);
                }

                // Updates the client Transfer Miles on the DataBase
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

                ViewBag.Message = "Your transfer was successful!";
                return View();
            }

            this.ModelState.AddModelError(string.Empty, "Please select a valid Client Id");
            return View(model);
        }

        public IActionResult ExtendMiles()
        {
            // Generates a new instance of ProlongMilesViewModel
            var model = new ExtendMilesViewModel
            {
                Amount = 2000
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExtendMiles(ExtendMilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Gets the current Client
                var client = await _clientRepository.GetByEmailAsync(this.User.Identity.Name);

                //Checks if the maximum value of transfered miles has been reached
                var prolongMiles = model.Amount + client.ProlongedMiles;

                if (prolongMiles > 20000)
                {
                    this.ModelState.AddModelError(string.Empty, "You can only extend a maximum of 20.000 Miles per Year");

                    return View(model);
                }

                // Gets a list of the client Bonus Miles
                var clientMiles =  _mileRepository.GetAllBonusMiles(client.Id);

                // Creates a new Mile Object for the giftedClient
                var mile = _converterHelper.ToMile(model, client.Id, 1);

                var extendedMiles = _converterHelper.ToTransaction(model, mile);


                // Creates a transaction on the Database
                try
                {
                    await _transactionRepository.CreateAsync(extendedMiles);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your extension.Please try again later");
                    return View(model);
                }


                // Extends the Miles
                var extendSuccess = await _mileRepository.SpendMilesAsync(clientMiles, model.Amount);

                if (!extendSuccess)
                {

                    this.ModelState.AddModelError(string.Empty, "There was a critical error with the extension algorithm. Please contact the Administrator as soon as possible");
                    return View(model);
                }

                // Creates a new Mile on the DataBase
                try
                {
                    await _mileRepository.CreateAsync(mile);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                // Updates the client Prolonged Miles on the DataBase
                try
                {
                    client.ProlongedMiles = prolongMiles;
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    this.ModelState.AddModelError(string.Empty, "There was an error processing your purchase.Please try again later");
                    return View(model);
                }

                ViewBag.Message = "Your extension of miles was successful!";
                return View();

            }

            // Generates a new instance of ExtendMilesViewModel
            var returnModel = new ExtendMilesViewModel
            {
                Amount = 2000
            };

            return View(model);
        }

        public IActionResult ConvertMiles()
        {
            // Generates a new instance of ProlongMilesViewModel
            var model = new ConvertMilesViewModel
            {
                BonusAmount = 2000,
                StatusAmount = 1000
            };

            return View(model);
        }

        #endregion

        #region AdditionalMethods

        public string AssignEditModelBackgroundPath()
        {
            if (this.User.IsInRole("Gold"))
            {
                return "/lib/ClientTemplate/img/status/Gold.jpg";
            }
            else if (this.User.IsInRole("Silver"))
            {
                return "/lib/ClientTemplate/img/status/Silver.jpg";
            }
            else
            {
               return "/lib/ClientTemplate/img/status/Basic.jpg";
            }
        }

        #endregion
    }
}