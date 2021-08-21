using StrokeForEgypt.Entity.CommonEntity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Entity.AccountEntity
{
    public class RefreshToken : BaseEntity
    {
        [ForeignKey("Account")]
        [DisplayName(nameof(Account))]
        public int Fk_Account { get; set; }

        [DisplayName("Account")]
        public Account Account { get; set; }

        [DisplayName("Token")]
        public string Token { get; set; }

        [DisplayName("Expires")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Expires { get; set; }

        [DisplayName("Created By Ip")]
        public string CreatedByIp { get; set; }

        [DisplayName("Revoked")]
        [DataType(DataType.DateTime)]
        public DateTime? Revoked { get; set; }

        [DisplayName("Revoked By Ip")]
        public string RevokedByIp { get; set; }

        [DisplayName("Replaced By Token")]
        public string ReplacedByToken { get; set; }

        [DisplayName("Reason Revoked")]
        public string ReasonRevoked { get; set; }

        [DisplayName("IsExpired")]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [DisplayName("IsRevoked")]
        public bool IsRevoked => Revoked != null;

        [DisplayName("IsActive")]
        public new bool IsActive => !IsRevoked && !IsExpired;
    }
}
