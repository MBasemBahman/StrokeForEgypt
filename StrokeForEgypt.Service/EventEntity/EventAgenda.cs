using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventAgendaModel : BaseEntityModel
    {
        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [DisplayName("Time")]
        [DataType(DataType.Time)]
        public string Time { get; set; }

        [DisplayName("Event Agenda Galleries")]
        public ICollection<EventAgendaGalleryModel> EventAgendaGalleries { get; set; }
    }
}
