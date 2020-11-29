using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;

namespace AirMiles.FrontOffice.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public EditViewModel ToEditViewModel(Client client, User user, string backgroundPath)
        {
            if (user.PhotoUrl == null)
            {
                user.PhotoUrl = $"~/images/Users/Default_User_Image.png";
            }


            var status = string.Empty;

            if(backgroundPath == "/lib/ClientTemplate/img/status/Gold.jpg")
            {
                status = "Gold";
            }
            else if(backgroundPath == "/lib/ClientTemplate/img/status/Silver.jpg")
            {
                status = "Silver";
            }
            else
            {
                status = "Basic";
            }

            return new EditViewModel
            {
                ClientID = client.Id,
                FullName = user.FullName,
                Status = status,
                Email = user.Email,
                BirthDate = user.BirthDate.ToShortDateString(),
                PhotoUrl = user.PhotoUrl,
                TotalFlights = client.TotalFlights,
                BoughtMiles = client.BoughtMiles.ToString(),
                ProlongedMiles = client.ProlongedMiles.ToString(),
                TransferedMiles = client.TransferedMiles.ToString(),
                RevisionMonth = client.RevisionMonth.ToString(),
                BackgroundPath = backgroundPath,
            };
        }

        public IEnumerable<Balance_MovementsViewModel> ToBalanceMovementsViewModel(List<Transaction> transactions)
        {
            var model = new List<Balance_MovementsViewModel>();

            foreach(var transaction in transactions)
            {
               var item = new Balance_MovementsViewModel
                {
                    TransactionType = transaction.Description,
                    Date = transaction.TransactionDate,
                    Amount = transaction.Value
                };

                model.Add(item);
            }

            return model;
        }

        public Mile ToMile(BuyMilesViewModel model, int clientId, int extraYears)
        {
            return new Mile
            {
                ClientId = clientId,
                Qtd = model.Amount,
                MilesTypeId = 2,
                ExpirationDate = DateTime.Now.AddYears(extraYears)
            };
        }

        public Mile ToMile(ExtendMilesViewModel model, int clientId, int extraYears)
        {
            return new Mile
            {
                ClientId = clientId,
                Qtd = model.Amount,
                MilesTypeId = 2,
                ExpirationDate = DateTime.Now.AddYears(extraYears)
            };
        }

        public Transaction ToTransaction(BuyMilesViewModel model, Mile mile)
        {
            return new Transaction
            {
                Description = "Buy",
                Value = mile.Qtd,
                TransactionDate = DateTime.Now,
                Price = model.Price,
                ClientID = mile.ClientId,
                IsAproved = true,
                IsCreditCard = false
            };
        }

        public Mile ToMile(int giftedClientId, int amount, int extraYears)
        {
            return new Mile
            {
                ClientId = giftedClientId,
                Qtd = amount,
                MilesTypeId = 2,
                ExpirationDate = DateTime.Now.AddYears(extraYears),
                IsAproved = true,
                IsDeleted = false,
            };
        }

        public Transaction ToTransaction(int giftedClientId, int qtd)
        {
            return  new Transaction
            {
                Description = "Transfered",
                ClientID = giftedClientId,
                TransactionDate = DateTime.Now,
                Price = 0,
                Value = qtd,
                IsAproved = true,
                IsCreditCard = false
            };
        }

        public Transaction ToTransaction(ExtendMilesViewModel model, Mile mile)
        {
            return new Transaction
            {
                Description = "Prolong",
                Value = mile.Qtd,
                TransactionDate = DateTime.Now,
                Price = 70,
                ClientID = mile.ClientId,
                IsAproved = true,
                IsCreditCard = true
            };
        }

        public MilesCardViewModel ToMilesCardViewModel(Client client, User user)
        {
            var idString = client.Id.ToString().PadLeft(9 - client.Id.ToString().Length);

            idString = idString.Replace(' ', '0');

            for (int i = 3; i <= idString.Length; i += 3)
            {
                idString = idString.Insert(i, " ");
                i++;
            }

            return new MilesCardViewModel
            {
                ClientID = idString,
                FullName = user.FullName,
                ValidUntil = (DateTime.DaysInMonth(DateTime.Now.Year, 12) + "/" + 12 + "/" + DateTime.Now.Year).ToString(),
                LastUpdated = DateTime.Today.ToShortDateString(),
                BgImage = "/lib/ClientTemplate/img/card/card_background.jpg",
                BackLogo = "/lib/ClientTemplate/img/card/logo_black.png"
            };
        }
    }
}
