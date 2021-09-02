using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class BookingMemberAttachmentFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_BookingMember { get; set; }
    }
}
