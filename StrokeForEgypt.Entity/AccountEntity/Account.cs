using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.CommonEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.AccountEntity
{
    public class Account : ImageEntity
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
        public string PasswordHash { get; set; }

        [DisplayName("Login Token")]
        public string LoginTokenHash { get; set; }

        [DisplayName("Verification Code")]
        public string VerificationCodeHash { get; set; }

        [DisplayName("IsVerified")]
        public bool IsVerified { get; set; } = false;

        [DisplayName("Account Devices")]
        public ICollection<AccountDevice> AccountDevices { get; set; }

        [DisplayName("Notification Accounts")]
        public ICollection<NotificationAccount> NotificationAccounts { get; set; }

        [DisplayName("Bookings")]
        public ICollection<Booking> Bookings { get; set; }

        [DisplayName("RefreshTokens")]
        public List<RefreshToken> RefreshTokens { get; set; }

        [DisplayName("Account Payments")]
        public List<AccountPayment> AccountPayments { get; set; }
    }
}
