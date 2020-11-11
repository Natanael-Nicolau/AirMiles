using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Mile : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }

        [Display(Name ="Quantity")]
        public int Qtd { get; set; }

        [Display(Name ="Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        public int MilesTypeId { get; set; }
        public MilesType MilesType { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
