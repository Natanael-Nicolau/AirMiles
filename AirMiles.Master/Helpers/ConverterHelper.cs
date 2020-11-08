using AirMiles.Master.Data.Entities;
using AirMiles.Master.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public DetailsViewModel ToDetailsViewModel(User user, string role)
        {
            return new DetailsViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate.ToShortDateString(),
                PhotoUrl = user.PhotoUrl,
                Role = role,
            };
        }

        public IndexViewModel ToIndexViewModel(User user)
        {
            return new IndexViewModel
            {
                FullName = user.FullName,
                Username = user.UserName,
                IsEmailConfirmed = user.EmailConfirmed
            };
        }
    }
}
