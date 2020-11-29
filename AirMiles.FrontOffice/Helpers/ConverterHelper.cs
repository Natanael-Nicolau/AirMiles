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
            if(user.PhotoUrl == null)
            {
                user.PhotoUrl = $"~/images/Users/Default_User_Image.png";
            }

            return new EditViewModel
            {
                ClientID = client.Id,
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate.ToShortDateString(),
                PhotoUrl = user.PhotoUrl,
                BoughtMiles = client.BoughtMiles.ToString(),
                ProlongedMiles = client.ProlongedMiles.ToString(),
                TransferedMiles = client.TransferedMiles.ToString(),
                RevisionMonth = client.RevisionMonth.ToString(),
                BackgroundPath = backgroundPath
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

        public Mile ToMile(BuyMilesViewModel model, int clientId)
        {
            return new Mile
            {
                ClientId = clientId,
                Qtd = model.Amount,
                MilesTypeId = 2,
                ExpirationDate = DateTime.Now.AddYears(3)
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

        public Mile ToMile(int giftedClientId, int amount)
        {
            return new Mile
            {
                ClientId = giftedClientId,
                Qtd = amount,
                MilesTypeId = 2,
                ExpirationDate = DateTime.Now.AddYears(3),
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


    }
}
