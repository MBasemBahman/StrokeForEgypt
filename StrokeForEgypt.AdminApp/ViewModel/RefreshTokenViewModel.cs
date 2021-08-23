using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class RefreshTokenFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Account { get; set; }
    }
}
