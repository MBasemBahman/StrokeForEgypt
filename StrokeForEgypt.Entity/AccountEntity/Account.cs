using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.CommonEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Password { get; set; }

        [DisplayName("Token")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Token { get; set; }

        [DisplayName("Login Token")]
        public string LoginToken { get; set; }

        [DisplayName("Last Active")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastActive { get; set; }

        [DisplayName("Account Devices")]
        public ICollection<AccountDevice> AccountDevices { get; set; }

        [DisplayName("Notification Accounts")]
        public ICollection<NotificationAccount> NotificationAccounts { get; set; }

        [DisplayName("Bookings")]
        public ICollection<Booking> Bookings { get; set; }

        [DisplayName("RefreshTokens")]
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
