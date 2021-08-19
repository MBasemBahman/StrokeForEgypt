using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.NewsEntity
{
    public class NewsGallery : AttachmentEntity
    {
        [ForeignKey("News")]
        [DisplayName(nameof(News))]
        public int Fk_News { get; set; }

        [DisplayName("News")]
        public News News { get; set; }
    }
}
