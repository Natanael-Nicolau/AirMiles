using System;
using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class Balance_MovementsViewModel
    {
        [Display(Name = "Type")]
        public string TransactionType { get; set; }

        [Display(Name = "Miles Amount")]
        public int Amount { get; set; }

        [Display(Name = "Movement Date")]
        public DateTime Date { get; set; }
    }
}
