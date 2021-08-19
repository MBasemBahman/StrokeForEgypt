using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingMemberAttachment : AttachmentEntityModel
    {
        [ForeignKey("BookingMember")]
        [DisplayName(nameof(BookingMember))]
        public int Fk_BookingMember { get; set; }

        [DisplayName("Booking Member")]
        public BookingMember BookingMember { get; set; }
    }
}
