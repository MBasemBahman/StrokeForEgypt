using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.API.Models.Accounts
{
    public class AuthenticateRequest
    {
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }


        [DisplayName("Password Hash")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string Password { get; set; }

        [DisplayName("Login Token")]
        public string LoginToken { get; set; }
    }
}