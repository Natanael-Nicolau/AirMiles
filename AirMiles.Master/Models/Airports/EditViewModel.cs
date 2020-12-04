using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Airports
{
    public class EditViewModel
    {
        [Required]
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("[A-Z]{3}")]
        public string IATA { get; set; }

        [Required]
        [Range(-90d, 90d)]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180d, 180d)]
        public decimal Longitude { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
