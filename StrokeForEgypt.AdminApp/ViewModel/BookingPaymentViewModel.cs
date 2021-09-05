using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class BookingPaymentFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Booking { get; set; }
        public int Fk_Account { get; set; }
    }
}
