using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.SponsorEntity
{
    public class SponsorType : BaseEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Sponsors")]
        public ICollection<Sponsor> Sponsors { get; set; }
    }
}
