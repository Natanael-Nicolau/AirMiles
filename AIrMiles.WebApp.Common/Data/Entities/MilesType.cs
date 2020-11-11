using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class MilesType : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }

        public string Description { get; set; }
    }
}
