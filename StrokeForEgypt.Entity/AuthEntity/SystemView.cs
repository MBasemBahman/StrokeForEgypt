﻿using StrokeForEgypt.Entity.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.AuthEntity
{
    public class SystemView : BaseEntity
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [DisplayName("Display Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string DisplayName { get; set; }

        [DisplayName("System Role Premissions")]
        public ICollection<SystemRolePremission> SystemRolePremissions { get; set; }
    }
}
