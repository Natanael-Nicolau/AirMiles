using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Models.Partners
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public bool IsAproved { get; set; }

        public string Name { get; set; }

        [Display(Name = "Star Alliance Member?")]
        public bool IsStarAlliance { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name ="Added Since")]
        public DateTime CreationDate { get; set; }


        [Display(Name = "Partner Website")]
        [DataType(DataType.Url)]
        public string WebsiteUrl { get; set; }

        public string Description { get; set; }
    }
}
