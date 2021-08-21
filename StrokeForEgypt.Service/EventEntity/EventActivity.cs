using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventActivityModel : ImageEntityModel
    {
        public int Fk_Event { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }
    }
}
