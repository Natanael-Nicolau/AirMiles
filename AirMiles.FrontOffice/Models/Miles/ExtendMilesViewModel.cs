using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.FrontOffice.Models.Account
{
    public class ExtendMilesViewModel
    {
        [Display(Name = "Miles")]
        public int Amount { get; set; }
    }
}
