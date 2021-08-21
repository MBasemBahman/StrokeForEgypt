using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.AccountEntity
{
    public class AccountDeviceModel : BaseEntityModel
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

        [DisplayName("Last Active")]
        [DataType(DataType.DateTime)]
        public string LastActive { get; set; }
    }
}
