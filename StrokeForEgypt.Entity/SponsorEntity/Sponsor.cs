using StrokeForEgypt.Entity.CommonEntity;
using StrokeForEgypt.Entity.EventEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.SponsorEntity
{
    public class Sponsor : ImageEntity
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int? Fk_Event { get; set; }

        [DisplayName("Event")]
        public Event Event { get; set; }

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
        public SponsorType SponsorType { get; set; }
    }
}
