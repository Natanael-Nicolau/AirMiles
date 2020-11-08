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
        [RegularExpression("[A-z]{2,50}", ErrorMessage = "Expression Malformed")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name", Prompt = "Last Name...")]
        [RegularExpression("[A-z]{2,50}", ErrorMessage = "Expression Malformed")]
        public string LastName { get; set; }

        [Required]
        [Display(Prompt = "Email...")]
        [EmailAddress]
        public string Email { get; set; }

        public string Username => Email;


        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Photo { get; set; }

        [Required]
        [Display(Name = "Birth Date", Prompt = "yyyy/mm/dd")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
