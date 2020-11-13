using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Miles
{
    public class RequestsIndexViewModel
    {
        public int RequestId { get; set; }

        [Display(Name ="Client")]
        public string ClientName { get; set; }

        [Display(Name = "Code")]
        public string RequestCode { get; set; }

        [Display(Name = "Partner")]
        public string PartnerName { get; set; }

        [Display(Name = "Miles")]
        public int MilesAmount { get; set; }
    }
}
