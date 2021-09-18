using StrokeForEgypt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class NotificationAccountFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Notification { get; set; }
    }
}
