using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }



        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Required]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}
