using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Flights
{
    public class CreateViewModel
    {
        public int Id { get; set; }

        //[Required]
        //[Display(Name = "Price")]
        //public decimal BasePrice { get; set; }
        [Required]
        [Display(Name = "Miles Price")]
        public int BaseMilesPrice { get; set; }


        [Required]
        [Display(Name ="Start Airport")]
        [Range(1, int.MaxValue, ErrorMessage ="Please select a starting Airport.")]
        public int StartAirportId { get; set; }
        public IEnumerable<SelectListItem> StartAirports { get; set; }

        [Required]
        [Display(Name = "End Airport")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an ending Airport.")]
        public int EndAirportId { get; set; }
        public IEnumerable<SelectListItem> EndAirports { get; set; }

        [Required]
        [Display(Name = "Flight Company")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a flight company.")]
        public int FlightCompanyId { get; set; }
        public IEnumerable<SelectListItem> FlightCompanies { get; set; }


        [Required]
        [Display(Name = "Start Time", Prompt = "yyyy/MM/dd HH:mm")]
        public DateTime FlightStart { get; set; }

        [Required]
        [Display(Name = "End Time", Prompt = "yyyy/MM/dd HH:mm")]
        public DateTime FlightEnd { get; set; }

    }
}
