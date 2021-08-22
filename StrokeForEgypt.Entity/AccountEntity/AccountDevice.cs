using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.AccountEntity
{
    public class AccountDevice : BaseEntity
    {
        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }

        [DisplayName("Notification Token")]
        [Required(ErrorMessage = "{0} is required")]
        public string NotificationToken { get; set; }

        [DisplayName("Device Type")]
        public string DeviceType { get; set; }

        [DisplayName("App Version")]
        public string AppVersion { get; set; }

        [DisplayName("Device Version")]
        public string DeviceVersion { get; set; }

        [DisplayName("Device Model")]
        public string DeviceModel { get; set; }
    }
}
