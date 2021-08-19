using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.MainDataEntity
{
    public class CityModel : BaseEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [ForeignKey("Country")]
        [DisplayName(nameof(Country))]
        public int Fk_Country { get; set; }

        [DisplayName("Country")]
        public CountryModel Country { get; set; }

        [DisplayName("Booking Members")]
        public ICollection<BookingMemberModel> BookingMembers { get; set; }
    }
}
