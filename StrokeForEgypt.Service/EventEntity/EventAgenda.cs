using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventAgendaModel : ImageEntityModel
    {
        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Short Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [DisplayName("Long Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string LongDescription { get; set; }

        [DisplayName("From Date")]
        [DataType(DataType.Date)]
        public string FromDate { get; set; }

        [DisplayName("From Time")]
        [DataType(DataType.Time)]
        public string FromTime { get; set; }

        [DisplayName("To Date")]
        [DataType(DataType.Date)]
        public string ToDate { get; set; }

        [DisplayName("To Time")]
        [DataType(DataType.Time)]
        public string ToTime { get; set; }

        [DisplayName("Event Agenda Galleries")]
        public ICollection<EventAgendaGalleryModel> EventAgendaGalleries { get; set; }
    }
}
