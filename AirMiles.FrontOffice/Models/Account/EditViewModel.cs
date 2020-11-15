using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class EditViewModel
    {
        [DisplayFormat(DataFormatString ="{0:D9}")]
        public int ClientID { get; set; }

        public string FullName { get; set; }

        [Required]
        [Display(Prompt = "Email...")]
        [EmailAddress]
        public string Email { get; set; }

        public string Username => Email;

        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Birth Date", Prompt = "yyyy/mm/dd")]
        public string BirthDate { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Bought Miles")]
        public string BoughtMiles { get; set; }

        [Required]
        [Display(Name = "Prolonged Miles")]
        public string ProlongedMiles { get; set; }

        [Required]
        [Display(Name = "Transfered Miles")]
        public string TransferedMiles { get; set; }

        [Required]
        [Display(Name = "Revision Month")]
        public string RevisionMonth { get; set; }

        public string BackgroundPath { get; set; }
    }
}
