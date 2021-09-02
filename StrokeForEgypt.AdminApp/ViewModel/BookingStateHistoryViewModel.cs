using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class BookingStateHistoryFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Booking { get; set; }
    }
}
