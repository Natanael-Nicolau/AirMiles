using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontEnd.Models.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
