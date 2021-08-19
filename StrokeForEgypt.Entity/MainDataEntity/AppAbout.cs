using StrokeForEgypt.Entity.CommonEntity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.MainDataEntity
{
    public class AppAbout : BaseEntity
    {
        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }

        [DisplayName("WhatsApp")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string WhatsApp { get; set; }

        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Facebook Url")]
        [DataType(DataType.Url)]
        [Url]
        public string FacebookUrl { get; set; }

        [DisplayName("Instagram Url")]
        [DataType(DataType.Url)]
        [Url]
        public string InstagramUrl { get; set; }

        [DisplayName("Website Url")]
        [DataType(DataType.Url)]
        [Url]
        public string WebsiteUrl { get; set; }

        [DisplayName("Android Min Version")]
        public string AndroidMinVersion { get; set; }

        [DisplayName("IOS Min Version")]
        public string IOSMinVersion { get; set; }

        [DisplayName("Is Andriod Down?")]
        public bool AndriodDown { get; set; } = false;

        [DisplayName("Is IOS Down?")]
        public bool IOSDown { get; set; } = false;

        [DisplayName("Down Message")]
        [DataType(DataType.MultilineText)]
        public string DownMessage { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}
