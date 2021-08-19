using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.SponsorEntity
{
    public class SponsorModel : ImageEntityModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Url")]
        [DataType(DataType.Url)]
        [Url]
        public string Url { get; set; }

        [ForeignKey("SponsorType")]
        [DisplayName(nameof(SponsorType))]
        public int Fk_SponsorType { get; set; }

        [DisplayName("Sponsor Type")]
        public SponsorTypeModel SponsorType { get; set; }
    }
}
