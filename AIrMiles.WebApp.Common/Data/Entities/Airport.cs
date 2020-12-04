using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Airport : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }


        public string Name { get; set; }
        public string IATA { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public string Region { get; set; }


        [NotMapped]
        [Display(Name = "Location")]
        public string FullLocation => $"{City}, {Country}";
    }
}
