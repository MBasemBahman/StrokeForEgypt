using StrokeForEgypt.Entity.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.MainDataEntity
{
    public class Country : ImageEntity
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Phone Code")]
        public string PhoneCode { get; set; }

        [DisplayName("Cities")]
        public ICollection<City> Cities { get; set; }
    }
}
