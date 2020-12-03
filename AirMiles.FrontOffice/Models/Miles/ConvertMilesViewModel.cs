using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class ConvertMilesViewModel
    {
        [Display(Name = "Bonus Miles")]
        public int BonusAmount { get; set; }

        [Display(Name = "Status Miles")]
        public int StatusAmount { get; set; }
    }
}
