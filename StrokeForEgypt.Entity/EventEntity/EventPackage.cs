using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.EventEntity
{
    public class EventPackage : ImageEntity
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Stay in Hotel")]
        public bool StayHotel { get; set; } = true;

        [DisplayName("Person Count")]
        public int PersonCount { get; set; } = 1;

        [DisplayName("Price")]
        public double Price { get; set; }

        [DisplayName("Extra Fees")]
        public double ExtraFees { get; set; }

        [DisplayName("Bookings")]
        public ICollection<Booking> Bookings { get; set; }
    }
}
