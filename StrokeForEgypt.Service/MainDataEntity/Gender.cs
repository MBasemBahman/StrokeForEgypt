﻿using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.MainDataEntity
{
    public class Gender : BaseEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Booking Members")]
        public ICollection<BookingMember> BookingMembers { get; set; }
    }
}
