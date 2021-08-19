using StrokeForEgypt.Service.CommonEntity;
using StrokeForEgypt.Service.MainDataEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrokeForEgypt.Service.BookingEntity
{
    public class BookingMemberModel : BaseEntityModel
    {
        [ForeignKey("Booking")]
        [DisplayName(nameof(Booking))]
        public int Fk_Booking { get; set; }

        [DisplayName("Booking")]
        public BookingModel Booking { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; }

        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [DisplayName("Phone")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey("Gender")]
        [DisplayName(nameof(Gender))]
        public int Fk_Gender { get; set; }

        [DisplayName("Gender")]
        public GenderModel Gender { get; set; }

        [ForeignKey("City")]
        [DisplayName(nameof(City))]
        public int Fk_City { get; set; }

        [DisplayName("City")]
        public CityModel City { get; set; }

        [DisplayName("Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DisplayName("Club")]
        public string Club { get; set; }

        [DisplayName("Date Of Birth")]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }

        [DisplayName("Member Attachments")]
        public ICollection<BookingMemberAttachmentModel> BookingMemberAttachments { get; set; }

        [DisplayName("Booking Member Activities")]
        public ICollection<BookingMemberActivityModel> BookingMemberActivities { get; set; }
    }
}
