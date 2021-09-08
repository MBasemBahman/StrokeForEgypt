using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int Order { get; set; }

        [DisplayName("Created At")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Created At")]
        [NotMapped]
        public string CreatedAtstring => CreatedAt.AddHours(2).ToString("ddd, dd/MM/yyyy h:mm tt");

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Last Modified At")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastModifiedAt { get; set; }

        [DisplayName("Last Modified At")]
        [NotMapped]
        public string LastModifiedAtString => LastModifiedAt.AddHours(2).ToString("ddd, dd/MM/yyyy h:mm tt");

        [DisplayName("Last Modified By")]
        public string LastModifiedBy { get; set; }
    }
}
