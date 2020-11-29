using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Clients
{
    public class DetailsViewModel
    {
        [DisplayFormat(DataFormatString = "{0:D9}")]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}")]
        [Display(Name = "Date of next Revision")]
        public DateTime NextRevision { get; set; }

        //public string PhotoUrl { get; set; }

        public int TotalStatusMiles { get; set; }
        public int TotalBonusMiles { get; set; }


        [Display(Name = "Bought Miles")]
        public int BoughtMiles { get; set; }

        [Display(Name = "Prolonged Miles")]
        public int ProlongedMiles { get; set; }

        [Display(Name = "Transfered Miles")]
        public int TransferedMiles { get; set; }

    }
}
