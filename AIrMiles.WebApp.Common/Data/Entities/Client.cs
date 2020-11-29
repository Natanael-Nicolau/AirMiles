using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Entities
{
    public class Client : IEntity
    {
        [DisplayFormat(DataFormatString = "{0:N9}")]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAproved { get; set; }



        [Display(Name = "Buy Cap")]
        public int BoughtMiles { get; set; }

        [Display(Name = "Prolong Cap")]
        public int ProlongedMiles { get; set; }

        [Display(Name = "Transfer Cap")]
        public int TransferedMiles { get; set; }

        [Display(Name = "Revision Month")]
        public int RevisionMonth { get; set; }

        [NotMapped]
        [Display(Name = "Date of next Revision")]
        public DateTime NextRevision
        {
            get
            {
                if (DateTime.Now.Month < this.RevisionMonth)
                {
                    return new DateTime(DateTime.Now.Year, this.RevisionMonth, 1);
                }
                else
                {
                    return new DateTime((DateTime.Now.Year + 1), this.RevisionMonth, 1);
                }
            }
        }

        [Display(Name = "Number of Flights")]
        public int TotalFlights { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }

        //For Gold Gifting
        public int? GifterId { get; set; }
        public Client Gifter { get; set; }
    }
}
