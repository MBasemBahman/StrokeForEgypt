using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class CityFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Country { get; set; }
    }
}
