using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public interface IPartnerRepository : IGenericRepository<Partner>
    {
        IEnumerable<SelectListItem> GetAllFlightCompanies();
    }
}
