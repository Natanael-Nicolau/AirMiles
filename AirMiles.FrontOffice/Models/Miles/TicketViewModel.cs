using System;
using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class TicketViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Client Name")]
        public string FullName { get; set; }

        [Display(Name = "First Airport")]
        public string StartAirport { get; set; }
        [Display(Name = "Last Airport")]
        public string EndAirport { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime FlightStart { get; set; }

        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime FlightEnd { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Class")]
        public string FlightClass { get; set; }

        [Display(Name = "Seat")]
        public string Seat { get; set; }
    }
}
