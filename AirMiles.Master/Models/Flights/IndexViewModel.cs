using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Flights
{
    public class IndexViewModel
    {
        public int Id { get; set; }

        public bool IsAproved { get; set; }

        [Display(Name = "Start Airport")]
        public string StartAirportName { get; set; }
        [Display(Name = "End Airport")]
        public string EndAirportName { get; set; }
        [Display(Name = "Company")]
        public string FlightCompanyName { get; set; }


        [Display(Name = "Start Time")]
        public DateTime FlightStart { get; set; }

    }
}
