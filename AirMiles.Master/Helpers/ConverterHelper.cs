using AirMiles.Master.Models.Miles;
using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Models.Account.DetailsViewModel ToDetailsViewModel(User user, string role)
        {
            return new Models.Account.DetailsViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate.ToShortDateString(),
                PhotoUrl = user.PhotoUrl,
                Role = role,
            };
        }

        public Models.Account.IndexViewModel ToIndexViewModel(User user)
        {
            return new Models.Account.IndexViewModel
            {
                FullName = user.FullName,
                Username = user.UserName,
                IsEmailConfirmed = user.EmailConfirmed
            };
        }

        public User ToUserEntity(Models.Account.CreateViewModel model, string photoPath)
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

        public Models.Account.EditViewModel ToEditViewModel(User user, string role)
        {
            return new Models.Account.EditViewModel
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

        public Flight ToFlightEntity(Models.Flights.CreateViewModel model)
        {
            return new Flight
            {
                StartAirportId = model.StartAirportId,
                EndAirportId = model.EndAirportId,
                FlightCompanyId = model.FlightCompanyId,
                FlightStart = model.FlightStart,
                FlightEnd = model.FlightEnd,
                BaseMilesPrice = model.BaseMilesPrice,
                IsAproved = false,
                IsDeleted = false
            };
        }

        public RequestsIndexViewModel ToRequestsIndexViewModel(MilesRequest request)
        {
            return new RequestsIndexViewModel
            {
                ClientName = request.Client.User.FullName,
                RequestCode = request.RequestCode,
                MilesAmount = request.MilesAmount,
                PartnerName = request.Partner.Name,
                RequestId = request.Id
            };
        }
    }
}
