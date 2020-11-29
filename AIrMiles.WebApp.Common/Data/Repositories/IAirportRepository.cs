using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public interface IAirportRepository : IGenericRepository<Airport>
    {
        IEnumerable<SelectListItem> GetStartingAirports();
        IEnumerable<SelectListItem> GetEndAirports(int StartAirportId);

        Task<Airport> GetByIataAsync(string iata);
    }
}
