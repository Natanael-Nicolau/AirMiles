using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class BuyMilesViewModel
    {
        [Display(Name = "Miles")]
        public int Amount { get; set; }

        [Display(Name = "Price")]
        public int Price { get; set; }

        public bool Selected { get; set; }
    }
}
