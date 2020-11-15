using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;

namespace AirMiles.FrontOffice.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public EditViewModel ToEditViewModel(Client client, User user)
        {
          
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
    }
}
