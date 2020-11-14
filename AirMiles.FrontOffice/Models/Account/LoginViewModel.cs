using System.ComponentModel.DataAnnotations;

namespace AirMiles.FrontOffice.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression("[0-9]{9}", ErrorMessage = "Incorrect Format! Client ID requires 9 digits")]
        [Display(Name = "Client ID", Prompt = "Ex: 245618830")]
        public string ClientId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
