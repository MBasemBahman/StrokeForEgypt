using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.MainDataEntity
{
    public class City : BaseEntity
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [ForeignKey("Country")]
        [DisplayName(nameof(Country))]
        public int Fk_Country { get; set; }

        [DisplayName("Country")]
        public Country Country { get; set; }

        [DisplayName("Booking Members")]
        public ICollection<BookingMember> BookingMembers { get; set; }
    }
}
