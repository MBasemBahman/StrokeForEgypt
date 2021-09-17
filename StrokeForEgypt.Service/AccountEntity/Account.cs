using Microsoft.AspNetCore.Http;
using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.AccountEntity
{
    public class AccountFullModel : ImageEntityModel
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

        [DisplayName("IsVerified")]
        public bool IsVerified { get; set; }
    }

    public class ForgetPasswordModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class CheckCodeModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "{0} is required")]
        public string Code { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "{0} is required")]
        public string Code { get; set; }

        [DisplayName("New Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string NewPassword { get; set; }
    }

    public class RegisterModel
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

        [DisplayName("Login Token")]
        public string LoginToken { get; set; }

        [DisplayName("Notification Token")]
        public string NotificationToken { get; set; }
    }

    public class NotificationTokenModel
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [DisplayName("Notification Token")]
        public string NotificationToken { get; set; }
    }

    public class ProfileImageModel
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [DisplayName("Profile Image")]
        public IFormFile ProfileImage { get; set; }
    }

    public class EditProfileModel
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

    public class ChangePasswordModel
    {
        [DisplayName("Old Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string OldPassword { get; set; }

        [DisplayName("New Password")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string NewPassword { get; set; }
    }

    public class VerifyAccountModel
    {
        [DisplayName("Code")]
        [Required(ErrorMessage = "{0} is required")]
        public string Code { get; set; }
    }
}
