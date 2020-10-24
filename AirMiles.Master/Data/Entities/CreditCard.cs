using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Data.Entities
{
    public class CreditCard : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }

        [Display(Name ="Expiration Date")]
        public DateTime ExpirationDate { get; set; }
        public string CVV { get; set; }

        [Display(Name ="Billing Address")]
        public string BillingAddress { get; set; }
    }
}
