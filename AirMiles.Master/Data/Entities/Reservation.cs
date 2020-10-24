using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Data.Entities
{
    public class Reservation : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }


        [Display(Name ="Reservation Date")]
        public DateTime ReservationDate { get; set; }
        public decimal Price { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int ReservationTypeId { get; set; }
        public ReservationType ReservationType { get; set; }
    }
}
