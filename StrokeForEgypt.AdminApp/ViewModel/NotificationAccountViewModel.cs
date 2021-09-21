using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class NotificationAccountFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Notification { get; set; }
    }
}
