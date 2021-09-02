using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.BookingEntity;
using System.Collections.Generic;

namespace StrokeForEgypt.AdminApp.ViewModel
{

    public class BookingMemberFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Booking { get; set; }
    }
    public class BookingMemberViewModel
    {
        public BookingMemberViewModel()
        {
            BookingMember = new BookingMember();
            Fk_Activities = new List<int>();
        }
        public BookingMember BookingMember { get; set; }
        public List<int> Fk_Activities { get; set; }
    }
}
