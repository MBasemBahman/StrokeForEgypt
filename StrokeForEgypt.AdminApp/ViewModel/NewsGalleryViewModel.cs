using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class NewsGalleryFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_News { get; set; }
    }
}
