using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingMemberActivityModel : BaseEntityModel
    {
        [ForeignKey("BookingMember")]
        [DisplayName(nameof(BookingMember))]
        public int Fk_BookingMember { get; set; }

        [DisplayName("Booking Member")]
        public BookingMemberModel BookingMember { get; set; }

        [ForeignKey("EventActivity")]
        [DisplayName(nameof(EventActivity))]
        public int Fk_EventActivity { get; set; }

        [DisplayName("Event Activity")]
        public EventActivityModel EventActivity { get; set; }
    }
}
