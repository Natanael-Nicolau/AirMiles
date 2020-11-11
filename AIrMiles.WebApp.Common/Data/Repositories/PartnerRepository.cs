using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class PartnerRepository : GenericRepository<Partner>, IPartnerRepository
    {
        private readonly DataContext _context;

        public PartnerRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
