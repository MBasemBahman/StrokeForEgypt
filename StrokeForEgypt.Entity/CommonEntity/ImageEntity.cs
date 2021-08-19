using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.CommonEntity
{
    public class ImageEntity : BaseEntity
    {
        [DisplayName("Image URL")]
        [DataType(DataType.ImageUrl)]
        [Url]
        public string ImageURL { get; set; }
    }
}
