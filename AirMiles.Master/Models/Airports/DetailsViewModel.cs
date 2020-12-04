using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Airports
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public bool IsAproved { get; set; }
        public string Name { get; set; }
        public string FullLocation { get; set; }
        public string IATA { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Region { get; set; }
    }
}
