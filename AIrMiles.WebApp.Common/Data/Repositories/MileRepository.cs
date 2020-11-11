using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
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
