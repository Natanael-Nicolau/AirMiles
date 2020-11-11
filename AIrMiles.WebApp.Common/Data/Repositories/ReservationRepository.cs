using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        private readonly DataContext _context;

        public ReservationRepository(DataContext context) : base(context)
        {
            _context = context;
        }

    }
}
