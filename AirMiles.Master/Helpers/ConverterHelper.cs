using AirMiles.Master.Models.Account;
using AIrMiles.WebApp.Common.Data.Entities;
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

        public User ToUserEntity(CreateViewModel model, string photoPath)
        {
            return new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email,
                BirthDate = model.BirthDate,
                PhotoUrl = photoPath
            };
        }

        public EditViewModel ToEditViewModel(User user, string role)
        {
            return new EditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                Role = role
            };
        }
    }
}
