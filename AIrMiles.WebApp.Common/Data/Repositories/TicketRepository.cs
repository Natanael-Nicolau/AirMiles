using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task DeleteOldTickets()
        {
            var ticketsToRemove = _context.Tickets
                .Include(t => t.Flight)
                .Where(t => t.Flight.FlightEnd <= DateTime.Now);

            foreach (var ticket in ticketsToRemove)
            {
                ticket.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
        }

        public IQueryable<Ticket> GetAllWithFlightInfo()
        {
            return _context.Tickets
                .Include(t => t.Flight)
                .ThenInclude(f => f.StartAirport)
                .Include(t => t.Flight)
                .ThenInclude(f => f.EndAirport)
                .Include(t => t.FlightClass)
                .Where(t => !t.IsDeleted)
                .AsNoTracking();
        }
    }
}
