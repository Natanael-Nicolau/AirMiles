using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        private readonly DataContext _context;

        public AirportRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
