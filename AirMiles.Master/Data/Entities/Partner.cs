using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Data.Entities
{
    public class Partner : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }


        public string Name { get; set; }

        public bool IsStarAlliance { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreationDate { get; set; }

        public string WebsiteUrl { get; set; }

        public string Description { get; set; }
    }
}
