﻿using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.MainDataEntity
{
    public class CountryModel : ImageEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Cities")]
        public ICollection<CityModel> Cities { get; set; }
    }
}
