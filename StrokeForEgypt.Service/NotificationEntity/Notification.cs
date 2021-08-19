using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.NotificationEntity
{
    public class NotificationModel : ImageEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int? Fk_Event { get; set; }

        [DisplayName("Event")]
        public EventModel Event { get; set; }

        [DisplayName("Heading")]
        [Required(ErrorMessage = "{0} is required")]
        public string Heading { get; set; }

        [DisplayName("Content")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [DisplayName("TargetID")]
        public string Target_Id { get; set; }

        [ForeignKey("NotificationType")]
        [DisplayName(nameof(NotificationType))]
        public int Fk_NotificationType { get; set; }

        [DisplayName("Notification Type")]
        public NotificationTypeModel NotificationType { get; set; }

        [ForeignKey("OpenType")]
        [DisplayName(nameof(OpenType))]
        public int Fk_OpenType { get; set; }

        [DisplayName("Open Type")]
        public OpenTypeModel OpenType { get; set; }

        [DisplayName("Notification Accounts")]
        public ICollection<NotificationAccountModel> NotificationAccounts { get; set; }
    }
}
