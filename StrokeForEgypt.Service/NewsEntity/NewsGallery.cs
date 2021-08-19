using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.NewsEntity
{
    public class NewsGalleryModel : AttachmentEntityModel
    {
        [ForeignKey("News")]
        [DisplayName(nameof(News))]
        public int Fk_News { get; set; }

        [DisplayName("News")]
        public NewsModel News { get; set; }
    }
}
