using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.NewsEntity
{
    public class NewsGallery : AttachmentEntityModel
    {
        [ForeignKey("News")]
        [DisplayName(nameof(News))]
        public int Fk_News { get; set; }

        [DisplayName("News")]
        public News News { get; set; }
    }
}
