using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class EventAgendaGalleryFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_EventAgenda { get; set; }
    }
}
