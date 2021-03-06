﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class EditViewModel
    {
        [DisplayFormat(DataFormatString ="{0:D9}")]
        public int ClientID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [Display(Prompt = "Email...")]
        [EmailAddress]
        public string Email { get; set; }

        public string Username => Email;

        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Birth Date", Prompt = "yyyy/mm/dd")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Status Miles")]
        public string StatusMiles { get; set; }

        [Required]
        [Display(Name = "Bonus Miles")]
        public string BonusMiles { get; set; }

        [Required]
        [Display(Name = "Bought Miles")]
        public string BoughtMiles { get; set; }

        [Required]
        [Display(Name = "Prolonged Miles")]
        public string ProlongedMiles { get; set; }

        [Required]
        [Display(Name = "Transfered Miles")]
        public string TransferedMiles { get; set; }

        [Required]
        [Display(Name = "Revision Month")]
        public string RevisionMonth { get; set; }

        [Display(Name = "Number of Flights")]
        public int TotalFlights { get; set; }

        public string BackgroundPath { get; set; }
    }
}
