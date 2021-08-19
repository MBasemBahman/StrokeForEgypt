using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.BookingEntity
{
    public class BookingMemberAttachment : AttachmentEntity
    {
        [ForeignKey("BookingMember")]
        [DisplayName(nameof(BookingMember))]
        public int Fk_BookingMember { get; set; }

        [DisplayName("Booking Member")]
        public BookingMember BookingMember { get; set; }
    }
}
