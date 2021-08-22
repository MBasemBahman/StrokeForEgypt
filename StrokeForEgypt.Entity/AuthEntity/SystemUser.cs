using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.AuthEntity
{
    public class SystemUser : BaseEntity
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string FullName { get; set; }

        [DisplayName("Job Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string JobTitle { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Password { get; set; }

        [ForeignKey("SystemRole")]
        [DisplayName(nameof(SystemRole))]
        public int Fk_SystemRole { get; set; }

        [DisplayName("System Role")]
        public SystemRole SystemRole { get; set; }
    }
}
