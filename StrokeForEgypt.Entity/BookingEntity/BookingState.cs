using StrokeForEgypt.Entity.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.BookingEntity
{
    public class BookingState : BaseEntity
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
