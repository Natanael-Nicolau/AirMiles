using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers.DailyUpdates
{
    public class DailyUpdateHelper : IDailyUpdateHelper
    {
        public async Task<int> CalculateTicketMilesAsync(TicketUpdateModel ticket)
        {
            //TODO: get algoritmo from internets

            return 7777;
        }
    }
}
