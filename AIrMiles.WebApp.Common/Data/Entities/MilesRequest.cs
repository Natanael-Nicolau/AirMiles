using System;
using System.Collections.Generic;
using System.Text;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class MilesRequest : IEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }


        public string RequestCode { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int MilesAmount { get; set; }

        public int PartnerId { get; set; }

        public Partner Partner { get; set; }
    }
}
