﻿using StrokeForEgypt.Service.AccountEntity;
using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class Booking : BaseEntityModel
    {
        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }

        [ForeignKey("EventPackage")]
        [DisplayName(nameof(EventPackage))]
        public int Fk_EventPackage { get; set; }

        [DisplayName("Event Package")]
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

        [DisplayName("Booking State Histories")]
        public ICollection<BookingStateHistory> BookingStateHistories { get; set; }

        [DisplayName("Booking Members")]
        public ICollection<BookingMember> BookingMembers { get; set; }
    }
}
