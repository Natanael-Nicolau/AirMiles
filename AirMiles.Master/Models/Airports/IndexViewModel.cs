using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Airports
{
    public class IndexViewModel
    {
        public int Id { get; set; }

        public bool IsAproved { get; set; }
        public string Name { get; set; }
        public string IATA { get; set; }

        [Display(Name = "Location")]
        public string FullLocation { get; set; }

    }
}
