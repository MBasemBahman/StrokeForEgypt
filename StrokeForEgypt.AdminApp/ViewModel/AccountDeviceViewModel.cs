using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class AccountDeviceFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Account { get; set; }
    }
}
