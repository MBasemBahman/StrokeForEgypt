using StrokeForEgypt.Common;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class NotificationFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_NotificationType { get; set; }
        public int Fk_Account { get; set; }
    }
}
