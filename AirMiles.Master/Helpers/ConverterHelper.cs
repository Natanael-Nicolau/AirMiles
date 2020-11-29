using AirMiles.Master.Models.Flights;
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

        public Models.Airports.EditViewModel ToEditViewModel(Airport airport)
        {
            return new Models.Airports.EditViewModel
            {
                Id = airport.Id,
                Name = airport.Name,
                IATA = airport.IATA,
                Latitude = airport.Latitude,
                Longitude = airport.Longitude,
                Country = airport.Country,
                City = airport.City
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

        public Models.Miles.RequestsIndexViewModel ToRequestsIndexViewModel(MilesRequest request)
        {
            return new Models.Miles.RequestsIndexViewModel
            {
                ClientName = request.Client.User.FullName,
                RequestCode = request.RequestCode,
                MilesAmount = request.MilesAmount,
                PartnerName = request.Partner.Name,
                RequestId = request.Id
            };
        }

        public Airport ToAirportEntity(Models.Airports.CreateViewModel model)
        {
            return new Airport
            {
                IsAproved = false,
                IsDeleted = false,
                IATA = model.IATA,
                Name = model.Name,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Country = model.Country,
                City = model.City
            };
        }

        public Partner ToPartnerEntity(Models.Partners.CreateViewModel model, string imageUrl)
        {
            return new Partner
            {
                IsAproved = false,
                IsDeleted = false,
                IsStarAlliance = model.IsStarAlliance,
                CreationDate = DateTime.Now,
                ImageUrl = imageUrl,
                Description = model.Description,
                Name = model.Name,
                WebsiteUrl = model.WebsiteUrl
            };
        }

        public Models.Partners.EditViewModel ToEditViewModel(Partner partner)
        {
            return new Models.Partners.EditViewModel
            {
                Id = partner.Id,
                ImagePath = partner.ImageUrl,
                Description = partner.Description,
                IsStarAlliance = partner.IsStarAlliance,
                Name = partner.Name,
                WebsiteUrl = partner.WebsiteUrl
            };
        }

        public Models.Flights.EditViewModel ToEditViewModel(Flight flight)
        {
            return new Models.Flights.EditViewModel
            {
                BaseMilesPrice = flight.BaseMilesPrice,
                StartAirportId = flight.StartAirportId,
                EndAirportId = flight.EndAirportId,
                FlightStart = DateTime.ParseExact(flight.FlightStart.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", null),
                FlightEnd = DateTime.ParseExact(flight.FlightEnd.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", null),
                FlightCompanyId = flight.FlightCompanyId,
                Id = flight.Id
            };
        }

        public Models.Airports.DetailsViewModel ToDetailsViewModel(Airport airport)
        {
            return new Models.Airports.DetailsViewModel
            {
                Name = airport.Name,
                IATA = airport.IATA,
                FullLocation = airport.FullLocation,
                Id = airport.Id,
                IsAproved = airport.IsAproved,
                Latitude = airport.Latitude,
                Longitude = airport.Longitude
            };
        }

        public Models.Partners.DetailsViewModel ToDetailsViewModel(Partner partner)
        {
            return new Models.Partners.DetailsViewModel
            {
                Name = partner.Name,
                IsStarAlliance = partner.IsStarAlliance,
                CreationDate = partner.CreationDate,
                Description = partner.Description,
                Id = partner.Id,
                ImageUrl = partner.ImageUrl,
                IsAproved = partner.IsAproved,
                WebsiteUrl = partner.WebsiteUrl
            };
        }

        public DetailsViewModel ToDetailsViewModel(Flight flight)
        {
            return new DetailsViewModel
            {
                Id = flight.Id,
                IsAproved = flight.IsAproved,
                StartAirportName = flight.StartAirport.Name,
                EndAirportName = flight.EndAirport.Name,
                FlightStart = flight.FlightStart,
                FlightEnd = flight.FlightEnd,
                FlightCompanyName = flight.FlightCompany.Name,
                BaseMilesPrice = flight.BaseMilesPrice
            };
        }

        public Models.Clients.DetailsViewModel ToDetailsViewModel(Client client, string status)
        {
            return new Models.Clients.DetailsViewModel
            {
                Id = client.Id,
                Status = status,
                Email = client.User.Email,
                FullName = client.User.FullName,
                NextRevision = client.NextRevision,
                BoughtMiles = client.BoughtMiles,
                ProlongedMiles = client.ProlongedMiles,
                TransferedMiles = client.TransferedMiles
            };
        }
    }
}
