using System;

namespace AirMiles.Master.Helpers.DailyUpdates
{
    public class TicketUpdateModel
    {
        public string ClientId { get; set; }
        public string Fullname { get; set; }
        public string StartRegion { get; set; }
        public string EndRegion { get; set; }
        public string StartIATA { get; set; }
        public string EndIATA { get; set; }
        public string FlightClass { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}