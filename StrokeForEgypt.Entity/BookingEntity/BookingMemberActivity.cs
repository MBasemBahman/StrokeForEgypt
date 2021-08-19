using StrokeForEgypt.Entity.CommonEntity;
using StrokeForEgypt.Entity.EventEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.BookingEntity
{
    public class BookingMemberActivity : BaseEntity
    {
        [ForeignKey("BookingMember")]
        [DisplayName(nameof(BookingMember))]
        public int Fk_BookingMember { get; set; }

        [DisplayName("Booking Member")]
        public BookingMember BookingMember { get; set; }

        [ForeignKey("EventActivity")]
        [DisplayName(nameof(EventActivity))]
        public int Fk_EventActivity { get; set; }

        [DisplayName("Event Activity")]
        public EventActivity EventActivity { get; set; }
    }
}
