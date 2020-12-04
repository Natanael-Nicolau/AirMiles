using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;

namespace AirMiles.FrontOffice.Helpers
{
    public interface IConverterHelper
    {
        EditViewModel ToEditViewModel(Client client, User user, string backgroundPath);

        IEnumerable<Balance_MovementsViewModel> ToBalanceMovementsViewModel(List<Transaction> transactions);

        IEnumerable<TicketViewModel> ToTicketIndexViewModel(User user, List<Ticket> tickets, List<Flight> flights);

        Mile ToMile(BuyMilesViewModel model, int clientId, int extraYears);

        Mile ToMile(ExtendMilesViewModel model, int clientId, DateTime newExpirationDate);

        Mile ToMile(ConvertMilesViewModel model, int clientId);
        Mile ToMile(int giftedClientId, int amount, int extraYears);



        Transaction ToTransaction(BuyMilesViewModel model, Mile mile);

        Transaction ToTransaction(ExtendMilesViewModel model, Mile mile);
        Transaction ToTransaction(ConvertMilesViewModel model, Mile mile);


        Transaction ToTransaction(int giftedClientId, int qtd);

        MilesCardViewModel ToMilesCardViewModel(Client client, User user);

    }
}
