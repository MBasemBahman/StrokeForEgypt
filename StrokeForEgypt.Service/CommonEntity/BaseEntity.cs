using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.CommonEntity
{
    public class BaseEntityModel
    {
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Created At")]
        [DataType(DataType.DateTime)]
        public string CreatedAt { get; set; }

        [DisplayName("Last Modified At")]
        [DataType(DataType.DateTime)]
        public string LastModifiedAt { get; set; }
    }
}
