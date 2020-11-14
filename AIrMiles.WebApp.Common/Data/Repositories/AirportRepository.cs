using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        private readonly DataContext _context;

        public AirportRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetEndAirports(int StartAirportId)
        {
            var list = _context.Airports
                .Where(a => a.Id != StartAirportId)
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                })
                .AsNoTracking()
                .ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "Please choose an ending airport...",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetStartingAirports()
        {
            var list = _context.Airports.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString()
            })
                .AsNoTracking()
                .ToList();


            list.Insert(0, new SelectListItem
            {
                Text = "Please choose a starting airport...",
                Value = "0"
            });

            return list;
        }
    }
}
