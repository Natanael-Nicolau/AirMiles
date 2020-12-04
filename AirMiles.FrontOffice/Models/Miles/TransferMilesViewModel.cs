using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class TransferMilesViewModel
    {
        [Display(Name = "Miles")]
        public int Amount { get; set; }

        [Required]
        [Display(Name = "Gifted Client Id")]
        [RegularExpression("[0-9]{9}", ErrorMessage = "Incorrect Format! Client ID requires 9 digits")]
        public string GiftedClientID { get; set; }
    }
}
