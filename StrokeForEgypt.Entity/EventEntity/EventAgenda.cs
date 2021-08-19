using StrokeForEgypt.Entity.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.EventEntity
{
    public class EventAgenda : BaseEntity
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayName("Time")]
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [DisplayName("Event Agenda Galleries")]
        public ICollection<EventAgendaGallery> EventAgendaGallerGalleries { get; set; }
    }
}
