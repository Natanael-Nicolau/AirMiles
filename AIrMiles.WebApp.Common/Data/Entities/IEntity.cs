using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public interface IEntity
    {
        int Id { get; set; }

        bool IsDeleted { get; set; }

        bool IsAproved { get; set; }
    }
}
