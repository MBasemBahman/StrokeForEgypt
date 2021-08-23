using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class SystemUserFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_SystemRole { get; set; }
    }
}
