using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.EventEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.NewsEntity
{
    public class NewsModel : ImageEntityModel
    {
        [ForeignKey("Event")]
        [DisplayName(nameof(Event))]
        public int? Fk_Event { get; set; }

        [DisplayName("Event")]
        public EventModel Event { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Short Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [DisplayName("Long Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string LongDescription { get; set; }

        [DisplayName("News Galleries")]
        public ICollection<NewsGalleryModel> NewsGalleries { get; set; }
    }
}
