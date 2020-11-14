using AirMiles.FrontOffice.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;

namespace AirMiles.FrontOffice.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public DetailsViewModel ToDetailsViewModel(Client client, User user)
        {
            return new DetailsViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate.ToShortDateString(),
                PhotoUrl = user.PhotoUrl,
                BoughtMiles = client.BoughtMiles.ToString(),
                ProlongedMiles = client.ProlongedMiles.ToString(),
                TransferedMiles = client.TransferedMiles.ToString(),
                RevisionMonth = client.RevisionMonth.ToString()
            };
        }
    }
}
