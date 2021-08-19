using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.BookingEntity
{
    public class BookingStateHistory : BaseEntity
    {
        [ForeignKey("Booking")]
        [DisplayName(nameof(Booking))]
        public int Fk_Booking { get; set; }

        [DisplayName("Booking")]
        public Booking Booking { get; set; }

        [ForeignKey("BookingState")]
        [DisplayName(nameof(BookingState))]
        public int Fk_BookingState { get; set; }

        [DisplayName("Booking State")]
        public BookingState BookingState { get; set; }
    }
}
