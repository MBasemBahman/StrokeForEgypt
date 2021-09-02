using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class SponsorFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Event { get; set; }
        public int Fk_SponsorType { get; set; }
    }
}
