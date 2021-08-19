using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventActivity : ImageEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Booking Member Activities")]
        public ICollection<BookingMemberActivity> BookingMemberActivities { get; set; }
    }
}
