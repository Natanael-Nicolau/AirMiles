using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Flights
{
    public class DetailsViewModel
    {
        public int Id { get; set; }

        public bool IsAproved { get; set; }
        //[Required]
        //[Display(Name = "Price")]
        //public decimal BasePrice { get; set; }

        [Display(Name = "Miles Price")]
        public int BaseMilesPrice { get; set; }


        [Display(Name = "Start Airport")]
        public string StartAirportName { get; set; }


        [Display(Name = "End Airport")]
        public string EndAirportName { get; set; }


        [Display(Name = "Flight Company")]
        public string FlightCompanyName { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "Start Time")]
        public DateTime FlightStart { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [Display(Name = "End Time")]
        public DateTime FlightEnd { get; set; }
    }
}
