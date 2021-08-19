using StrokeForEgypt.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.CommonEntity
{
    public class BaseEntity
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
        public DateTime CreatedAt { get; set; } = CurrentTime.Egypt();

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Last Modified At")]
        [DataType(DataType.DateTime)]
        public DateTime LastModifiedAt { get; set; } = CurrentTime.Egypt();

        [DisplayName("Last Modified By")]
        public string LastModifiedBy { get; set; }
    }
}
