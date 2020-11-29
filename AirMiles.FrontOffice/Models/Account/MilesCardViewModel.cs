using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.FrontOffice.Models.Account
{
    public class MilesCardViewModel
    {
        public string ClientID { get; set; }

        public string FullName { get; set; }

        [Display(Name = "Valid Until")]
        public string ValidUntil { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Status Miles")]
        public string StatusMiles { get; set; }

        [Display(Name = "Bonus Miles")]
        public string BonusMiles { get; set; }

        [Display(Name = "Last Updated")]
        public string LastUpdated { get; set; }

        public string BgImage { get; set; }

        public string StatusPhoto { get; set; }

        public string BackColor { get; set; }

        public string BackLogo { get; set; }
    }
}
