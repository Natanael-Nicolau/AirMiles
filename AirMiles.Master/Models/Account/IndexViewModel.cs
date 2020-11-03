using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Account
{
    public class IndexViewModel
    {
        [Required]
        public string Username { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }        

        [Display(Name ="Email Confirmed?")]
        public bool IsEmailConfirmed { get; set; }

        public string Position { get; set; }
    }
}
