using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.AccountEntity
{
    public class AccountDeviceModel
    {
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
