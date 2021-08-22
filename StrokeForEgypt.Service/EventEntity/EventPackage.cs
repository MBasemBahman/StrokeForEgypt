using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventPackageModel : ImageEntityModel
    {
        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("OriginalPrice")]
        public double OriginalPrice { get; set; }

        [DisplayName("Price")]
        public double Price { get; set; }

        [DisplayName("Extra Fees")]
        public double ExtraFees { get; set; }
    }
}
