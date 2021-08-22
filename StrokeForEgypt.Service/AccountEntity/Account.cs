using StrokeForEgypt.Service.CommonEntity;
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
    }
}
