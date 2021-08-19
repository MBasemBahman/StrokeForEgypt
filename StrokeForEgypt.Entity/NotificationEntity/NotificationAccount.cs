using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.NotificationEntity
{
    public class NotificationAccount : BaseEntity
    {
        [ForeignKey("Notification")]
        [DisplayName(nameof(Notification))]
        public int Fk_Notification { get; set; }

        [DisplayName("Notification Type")]
        public Notification Notification { get; set; }

        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }
    }
}
