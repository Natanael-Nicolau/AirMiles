using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Transaction : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }


        public int Value { get; set; }

        public string Description { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }


        public bool IsCreditCard { get; set; }
        public int? CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; }
        public decimal Price { get; set; }


        public int ClientID { get; set; }
        public Client Client { get; set; }

    }
}
