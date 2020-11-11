using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }

        [Display(Name = "Price")]
        public decimal BasePrice { get; set; }
        [Display(Name = "Miles Price")]
        public decimal BaseMilesPrice { get; set; }

        public int StartAirportId { get; set; }
        public int EndAirportId { get; set; }
        
        [Display(Name ="Start Time")]
        public DateTime FlightStart { get; set; }

        [Display(Name ="End Time")]
        public DateTime FlightEnd { get; set; }

        //Parter ID
        public int FlightCompanyId { get; set; }


        public Airport StartAirport { get; set; }
        public Airport EndAirport { get; set; }
        public Partner FlightCompany { get; set; }
    }
}
