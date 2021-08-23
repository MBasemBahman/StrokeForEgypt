using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class EventAgendaFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Event { get; set; }
    }
}
