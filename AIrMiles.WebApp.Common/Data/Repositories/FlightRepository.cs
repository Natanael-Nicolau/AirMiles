﻿using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Flight> GetAllWithAirportsAndPartners()
        {
            return _context.Flights
                .Where(f => !f.IsDeleted)
                .Include(f => f.StartAirport)
                .Include(f => f.EndAirport)
                .Include(f => f.FlightCompany)
                .AsNoTracking();
        }

        public async Task<Flight> GetByIdWithAirportsAndPartnersAsync(int flightId)
        {
            return await _context.Flights
                .Where(f => !f.IsDeleted && f.Id == flightId)
                .Include(f => f.StartAirport)
                .Include(f => f.EndAirport)
                .Include(f => f.FlightCompany)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<FlightClass> GetFlightClassByIdAsync(int id)
        {
            return await _context.FlightClasses
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
