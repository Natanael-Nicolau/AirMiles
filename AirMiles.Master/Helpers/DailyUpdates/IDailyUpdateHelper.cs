using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers.DailyUpdates
{
    public interface IDailyUpdateHelper
    {
        Task<int> CalculateTicketMilesAsync(TicketUpdateModel ticket);
    }
}
