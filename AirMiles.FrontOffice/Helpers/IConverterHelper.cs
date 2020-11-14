using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;

namespace AirMiles.FrontOffice.Helpers
{
    public interface IConverterHelper
    {
        DetailsViewModel ToDetailsViewModel(Client client, User user);
    }
}
