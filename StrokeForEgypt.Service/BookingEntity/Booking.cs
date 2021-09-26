using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingModel : BaseEntityModel
    {
        [ForeignKey("EventPackage")]
        [DisplayName(nameof(EventPackage))]
        public int Fk_EventPackage { get; set; }

        [DisplayName("Event Package")]
        public EventPackageModel EventPackage { get; set; }

        [DisplayName("Person Count")]
        public int PersonCount { get; set; }

        [DisplayName("Guest Count")]
        public int GuestCount { get; set; }

        [DisplayName("Days Count")]
        public int DaysCount { get; set; }

        [DisplayName("Total Price")]
        public double TotalPrice { get; set; }

        [ForeignKey("BookingState")]
        [DisplayName(nameof(BookingState))]
        public int Fk_BookingState { get; set; }

        [DisplayName("Booking State")]
        public BookingStateModel BookingState { get; set; }
    }

    public class BookingCreateModel
    {
        [DisplayName("Event Package")]
        public int Fk_EventPackage { get; set; }

        [DisplayName("Person Count")]
        public int PersonCount { get; set; }

        [DisplayName("Guest Count")]
        public int GuestCount { get; set; }

        [DisplayName("Days Count")]
        public int DaysCount { get; set; }

        [DisplayName("Total Price")]
        public double TotalPrice { get; set; }
    }
}
