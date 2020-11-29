using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Partners
{
    public class IndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name = "Star Alliance Member?")]
        public bool IsStarAlliance { get; set; }

        [Display(Name ="Added since")]
        [DisplayFormat(DataFormatString ="{0:d}")]
        public DateTime CreationDate { get; set; }

        public bool IsAproved { get; set; }
    }
}
