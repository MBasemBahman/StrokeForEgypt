using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingState : BaseEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Bookings")]
        public ICollection<Booking> Bookings { get; set; }

        [DisplayName("Booking State Histories")]
        public ICollection<BookingStateHistory> BookingStateHistories { get; set; }
    }
}
