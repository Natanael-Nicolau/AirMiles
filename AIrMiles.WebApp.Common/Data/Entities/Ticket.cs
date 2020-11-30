using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Ticket : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }

        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Seat { get; set; }


        public decimal Price { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int FlightClassId { get; set; }
        public FlightClass FlightClass { get; set; }

        public int FlightId { get; set; }
        public Flight Flight { get; set; }
    }
}
