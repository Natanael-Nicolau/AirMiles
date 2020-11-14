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
        User ToUserEntity(Models.Account.CreateViewModel model, string photoPath);

        Models.Account.EditViewModel ToEditViewModel(User user, string role);

        Flight ToFlightEntity(Models.Flights.CreateViewModel model);
    }
}
