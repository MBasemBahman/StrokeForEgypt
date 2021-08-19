using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.AuthEntity
{
    public class SystemRolePremission : BaseEntity
    {
        [ForeignKey("SystemRole")]
        [DisplayName(nameof(SystemRole))]
        public int Fk_SystemRole { get; set; }

        [DisplayName("System Role")]
        public SystemRole SystemRole { get; set; }

        [ForeignKey("AccessLevel")]
        [DisplayName(nameof(AccessLevel))]
        public int Fk_AccessLevel { get; set; }

        [DisplayName("Access Level")]
        public AccessLevel AccessLevel { get; set; }

        [ForeignKey("SystemView")]
        [DisplayName(nameof(SystemView))]
        public int Fk_SystemView { get; set; }

        [DisplayName("System View")]
        public SystemView SystemView { get; set; }
    }
}
