using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingStateHistoryModel : BaseEntityModel
    {
        [ForeignKey("Booking")]
        [DisplayName(nameof(Booking))]
        public int Fk_Booking { get; set; }

        [DisplayName("Booking")]
        public BookingModel Booking { get; set; }

        [ForeignKey("BookingState")]
        [DisplayName(nameof(BookingState))]
        public int Fk_BookingState { get; set; }

        [DisplayName("Booking State")]
        public BookingStateModel BookingState { get; set; }
    }
}
