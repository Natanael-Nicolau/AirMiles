using AirMiles.Master.Data.Entities;
using AirMiles.Master.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers
{
    public interface IConverterHelper
    {
        IndexViewModel ToIndexViewModel(User user);

        DetailsViewModel ToDetailsViewModel(User user, string role);
        User ToUserEntity(CreateViewModel model, string photoPath);

        EditViewModel ToEditViewModel(User user, string role);
    }
}
