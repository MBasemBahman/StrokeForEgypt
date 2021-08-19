using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventGallery : AttachmentEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }
    }
}
