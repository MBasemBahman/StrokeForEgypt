using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingStateModel : BaseEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Bookings")]
        public ICollection<BookingModel> Bookings { get; set; }

        [DisplayName("Booking State Histories")]
        public ICollection<BookingStateHistoryModel> BookingStateHistories { get; set; }
    }
}
