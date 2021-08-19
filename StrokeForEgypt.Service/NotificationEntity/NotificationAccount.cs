using StrokeForEgypt.Service.AccountEntity;
using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.NotificationEntity
{
    public class NotificationAccountModel : BaseEntityModel
    {
        [ForeignKey("Notification")]
        [DisplayName(nameof(Notification))]
        public int Fk_Notification { get; set; }

        [DisplayName("Notification Type")]
        public NotificationModel Notification { get; set; }

        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public AccountModel Account { get; set; }
    }
}
