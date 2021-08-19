using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.CommonEntity
{
    public class BaseEntityModel
    {
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("IsActive")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Order")]
        public int Order { get; set; } = 0;

        [DisplayName("Created At")]
        [DataType(DataType.DateTime)]
        public string CreatedAt { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Last Modified At")]
        [DataType(DataType.DateTime)]
        public string LastModifiedAt { get; set; }

        [DisplayName("Last Modified By")]
        public string LastModifiedBy { get; set; }
    }
}
