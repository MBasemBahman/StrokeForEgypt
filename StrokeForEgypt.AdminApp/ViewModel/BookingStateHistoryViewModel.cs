using StrokeForEgypt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class BookingStateHistoryFilter : DtParameters
    {
        public int Id { get; set; }
        public int Fk_Booking { get; set; }
    }
}
