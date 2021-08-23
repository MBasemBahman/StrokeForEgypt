using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class EventPackageFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Event { get; set; }
    }
}
