using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class BookingFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Account { get; set; }
        public int Fk_EventPackage { get; set; }
        public int Fk_BookingState { get; set; }
    }
}
