using AirMiles.Master.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Account
{
    public class DetailsViewModel
    {
        public string FullName { get; set; }

        [Required]
        [Display(Prompt = "Email...")]
        [EmailAddress]
        public string Email { get; set; }

        public string Username => Email;

        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Birth Date", Prompt = "yyyy/mm/dd")]
        public string BirthDate { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
