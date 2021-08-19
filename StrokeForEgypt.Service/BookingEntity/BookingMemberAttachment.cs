using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingMemberAttachmentModel : AttachmentEntityModel
    {
        [ForeignKey("BookingMember")]
        [DisplayName(nameof(BookingMember))]
        public int Fk_BookingMember { get; set; }

        [DisplayName("Booking Member")]
        public BookingMemberModel BookingMember { get; set; }
    }
}
