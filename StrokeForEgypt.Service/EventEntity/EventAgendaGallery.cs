using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventAgendaGallery : AttachmentEntityModel
    {
        [ForeignKey("EventAgenda")]
        [DisplayName(nameof(EventAgenda))]
        public int Fk_EventAgenda { get; set; }

        [DisplayName("Event Agenda")]
        public EventAgenda EventAgenda { get; set; }
    }
}
