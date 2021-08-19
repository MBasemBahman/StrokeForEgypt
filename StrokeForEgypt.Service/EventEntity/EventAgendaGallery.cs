using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventAgendaGalleryModel : AttachmentEntityModel
    {
        [ForeignKey("EventAgenda")]
        [DisplayName(nameof(EventAgenda))]
        public int Fk_EventAgenda { get; set; }

        [DisplayName("Event Agenda")]
        public EventAgendaModel EventAgenda { get; set; }
    }
}
