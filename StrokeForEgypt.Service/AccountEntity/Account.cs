using StrokeForEgypt.Service.CommonEntity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.AccountEntity
{
    public class AccountModel : ImageEntityModel
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string FullName { get; set; }

        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string Password { get; set; }

        [DisplayName("Token")]
        public Guid Token { get; set; }

        [DisplayName("Login Token")]
        public string LoginToken { get; set; }

        [DisplayName("Last Active")]
        [DataType(DataType.DateTime)]
        public string LastActive { get; set; }
    }
}
