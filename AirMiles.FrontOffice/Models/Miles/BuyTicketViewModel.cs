using System;
using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class BuyTicketViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Airport Name")]
        public string StartAirport { get; set; }
        [Display(Name = "Last Airport Name")]
        public string EndAirport { get; set; }

        [Display(Name = "Start Time")]
        public DateTime FlightStart { get; set; }

        [Display(Name = "End Time")]
        public DateTime FlightEnd { get; set; }

        public string Company { get; set; }

        [Display(Name = "Base Price")]
        public int Price { get; set; }

        public string FlightClass { get; set; }

        public bool Selected { get; set; }
    }
}
