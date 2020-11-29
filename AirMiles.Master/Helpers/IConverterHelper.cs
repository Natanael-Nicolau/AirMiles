using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers
{
    public interface IConverterHelper
    {
        Models.Account.IndexViewModel ToIndexViewModel(User user);
        Models.Miles.RequestsIndexViewModel ToRequestsIndexViewModel(MilesRequest request);

        Models.Account.DetailsViewModel ToDetailsViewModel(User user, string role);
        Models.Airports.DetailsViewModel ToDetailsViewModel(Airport airport);
        Models.Partners.DetailsViewModel ToDetailsViewModel(Partner partner);
        Models.Flights.DetailsViewModel ToDetailsViewModel(Flight flight);
        Models.Clients.DetailsViewModel ToDetailsViewModel(Client client, string status);


        Models.Account.EditViewModel ToEditViewModel(User user, string role);
        Models.Airports.EditViewModel ToEditViewModel(Airport airport);
        Models.Partners.EditViewModel ToEditViewModel(Partner partner);
        Models.Flights.EditViewModel ToEditViewModel(Flight flight);
        




        Flight ToFlightEntity(Models.Flights.CreateViewModel model);
        User ToUserEntity(Models.Account.CreateViewModel model, string photoPath);

        Airport ToAirportEntity(Models.Airports.CreateViewModel model);
        Partner ToPartnerEntity(Models.Partners.CreateViewModel model, string imageUrl);
        
    }
}
