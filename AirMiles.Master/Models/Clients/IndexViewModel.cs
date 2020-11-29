using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Clients
{
    public class IndexViewModel
    {
        [DisplayFormat(DataFormatString ="{0:D9}")]
        public int Id { get; set; }

        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }
    }
}
