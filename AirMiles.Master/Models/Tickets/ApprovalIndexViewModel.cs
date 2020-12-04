using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Tickets
{
    public class ApprovalIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name ="Start Airport")]
        public string StartIATA { get; set; }

        [Display(Name = "End Airport")]
        public string EndIATA { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? StartTime { get; set; }


        public string Class { get; set; }
    }
}
