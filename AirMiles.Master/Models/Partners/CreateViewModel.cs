using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Partners
{
    public class CreateViewModel
    {
        [Required]
        public string Name { get; set; }


        [Display(Name ="Star Alliance Member?")]
        public bool IsStarAlliance { get; set; }

        [Display(Name ="Company Logo")]
        public IFormFile ImageFile { get; set; }


        [Display(Name = "Partner Website")]
        [Required]
        [DataType(DataType.Url)]
        public string WebsiteUrl { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }
    }
}
