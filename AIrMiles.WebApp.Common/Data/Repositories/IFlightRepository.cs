﻿using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {

        IQueryable<Flight> GetAllWithAirportsAndPartners();
        Task<Flight> GetByIdWithAirportsAndPartnersAsync(int flightId);
        Task<FlightClass> GetFlightClassByIdAsync(int id);
    }
}
