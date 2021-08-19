using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.NotificationEntity;
using System;
using System.Collections.Generic;
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

        [DisplayName("Account Devices")]
        public ICollection<AccountDeviceModel> AccountDevices { get; set; }

        [DisplayName("Notification Accounts")]
        public ICollection<NotificationAccountModel> NotificationAccounts { get; set; }

        [DisplayName("Bookings")]
        public ICollection<BookingModel> Bookings { get; set; }
    }
}
