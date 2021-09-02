using StrokeForEgypt.Entity.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.EventEntity
{
    public class EventAgenda : ImageEntity
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }

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
        public DateTime FromDate { get; set; }

        [DisplayName("From Time")]
        [DataType(DataType.Time)]
        public TimeSpan FromTime { get; set; }

        [DisplayName("To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [DisplayName("To Time")]
        [DataType(DataType.Time)]
        public TimeSpan ToTime { get; set; }

        [DisplayName("Event Agenda Galleries")]
        public ICollection<EventAgendaGallery> EventAgendaGalleries { get; set; }
    }
}
