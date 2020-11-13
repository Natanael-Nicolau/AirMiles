using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class MilesRequestRepository : GenericRepository<MilesRequest>, IMilesRequestRepository
    {
        private readonly DataContext _context;

        public MilesRequestRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
