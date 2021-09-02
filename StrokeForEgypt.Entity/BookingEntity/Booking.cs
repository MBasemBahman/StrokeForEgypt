using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.CommonEntity;
using StrokeForEgypt.Entity.EventEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.BookingEntity
{
    public class Booking : BaseEntity
    {
        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }

        [ForeignKey("EventPackage")]
        [DisplayName(nameof(EventPackage))]
        public int Fk_EventPackage { get; set; }

        [DisplayName("EventPackage")]
        public EventPackage EventPackage { get; set; }

        [DisplayName("Person Count")]
        public int PersonCount { get; set; }

        [DisplayName("Days Count")]
        public int DaysCount { get; set; }

        [DisplayName("Total Price")]
        public double TotalPrice { get; set; }

        [ForeignKey("BookingState")]
        [DisplayName(nameof(BookingState))]
        public int Fk_BookingState { get; set; }

        [DisplayName("Booking State")]
        public BookingState BookingState { get; set; }

        [DisplayName("Admin Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Booking State Histories")]
        public ICollection<BookingStateHistory> BookingStateHistories { get; set; }

        [DisplayName("Booking Members")]
        public ICollection<BookingMember> BookingMembers { get; set; }

        [DisplayName("Booking Payments")]
        public List<BookingPayment> BookingPayments { get; set; }
    }
}
