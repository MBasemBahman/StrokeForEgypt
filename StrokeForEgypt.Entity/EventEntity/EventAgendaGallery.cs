using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.EventEntity
{
    public class EventAgendaGallery : AttachmentEntity
    {
        [ForeignKey("EventAgenda")]
        [DisplayName(nameof(EventAgenda))]
        public int Fk_EventAgenda { get; set; }

        [DisplayName("Event Agenda")]
        public EventAgenda EventAgenda { get; set; }
    }
}
