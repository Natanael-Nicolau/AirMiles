using AirMiles.Master.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Data.Repositories
{
    public class MileRepository : GenericRepository<Mile>, IMileRepository
    {
        private readonly DataContext _context;

        public MileRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
