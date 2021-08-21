using StrokeForEgypt.Service.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.AccountEntity
{
    public class RefreshTokenModel : BaseEntityModel
    {
        [DisplayName("Token")]
        public string Token { get; set; }

        [DisplayName("Expires")]
        [DataType(DataType.DateTime)]
        public string Expires { get; set; }

        [DisplayName("Created By Ip")]
        public string CreatedByIp { get; set; }

        [DisplayName("Revoked")]
        [DataType(DataType.DateTime)]
        public string Revoked { get; set; }

        [DisplayName("Revoked By Ip")]
        public string RevokedByIp { get; set; }

        [DisplayName("Replaced By Token")]
        public string ReplacedByToken { get; set; }

        [DisplayName("Reason Revoked")]
        public string ReasonRevoked { get; set; }

        [DisplayName("IsExpired")]
        public bool IsExpired { get; set; }

        [DisplayName("IsRevoked")]
        public bool IsRevoked { get; set; }

        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
    }
}
