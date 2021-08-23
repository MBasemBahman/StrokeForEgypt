using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class EventActivityFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Event { get; set; }
    }
}
