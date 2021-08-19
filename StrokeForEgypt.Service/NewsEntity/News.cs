using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.NewsEntity
{
    public class News : ImageEntityModel
    {
        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("News Galleries")]
        public ICollection<NewsGallery> NewsGalleries { get; set; }
    }
}
