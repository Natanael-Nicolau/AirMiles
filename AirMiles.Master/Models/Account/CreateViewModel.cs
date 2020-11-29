using AirMiles.Master.CustomValidators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirMiles.Master.Models.Account
{
    public class CreateViewModel
    {
        [Required]
        [Display(Name = "First Name", Prompt = "First Name...")]
        [RegularExpression("[a-zA-Z\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u024F]{2,50}", ErrorMessage = "Invalid Characters used")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name", Prompt = "Last Name...")]
        [RegularExpression("[a-zA-Z\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u024F]{2,50}", ErrorMessage = "Invalid Characters used")]
        public string LastName { get; set; }

        [Required]
        [Display(Prompt = "Email...")]
        [EmailAddress]
        public string Email { get; set; }

        public string Username => Email;


        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Photo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
