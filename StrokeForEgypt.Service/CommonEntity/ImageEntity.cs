using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.CommonEntity
{
    public class ImageEntityModel : BaseEntityModel
    {
        [DisplayName("Image URL")]
        [DataType(DataType.ImageUrl)]
        [Url]
        public string ImageURL { get; set; }
    }
}
