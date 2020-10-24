using AirMiles.Master.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Data.Repositories
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
