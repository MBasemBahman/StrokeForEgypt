using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.API.Models.Accounts
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }


        [DisplayName("Password Hash")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}