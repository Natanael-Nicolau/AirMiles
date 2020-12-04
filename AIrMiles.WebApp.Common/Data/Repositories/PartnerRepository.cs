using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<SelectListItem> GetAllFlightCompanies()
        {
            var list = _context.Partners
                .Where(p => p.IsAproved)
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                })
                .AsNoTracking()
                .ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "Please choose a company to fly with...",
                Value = "0"
            });

            return list;
        }

        public async Task<Partner> GetByNameAsync(string name)
        {
            return await _context.Partners
                .Where(p => p.Name == name)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        }
    }
}
