using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventPackageModel : ImageEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int Fk_Event { get; set; }

        [DisplayName("Event")]
        public EventModel Event { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Stay in Hotel")]
        public bool StayHotel { get; set; }

        [DisplayName("Person Count")]
        public int PersonCount { get; set; }

        [DisplayName("Price")]
        public double Price { get; set; }

        [DisplayName("Extra Fees")]
        public double ExtraFees { get; set; }
    }
}
