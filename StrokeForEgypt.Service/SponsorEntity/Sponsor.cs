using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.SponsorEntity
{
    public class SponsorModel : ImageEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int? Fk_Event { get; set; }

        [DisplayName("Event")]
        public EventModel Event { get; set; }

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
