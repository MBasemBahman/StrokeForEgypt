using StrokeForEgypt.Service.CommonEntity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Service.EventEntity
{
    public class EventModel : ImageEntityModel
    {
        [DisplayName("Title")]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; }

        [DisplayName("Short Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [DisplayName("Long Description")]
        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.MultilineText)]
        public string LongDescription { get; set; }

        [DisplayName("From Date")]
        [DataType(DataType.Date)]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        [DataType(DataType.Date)]
        public string ToDate { get; set; }

        [DisplayName("Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DisplayName("Latitude")]
        public double Latitude { get; set; }

        [DisplayName("Longitude")]
        public double Longitude { get; set; }

        [DisplayName("Registration From")]
        [DataType(DataType.Date)]
        public string RegistrationFrom { get; set; }

        [DisplayName("Registration To")]
        [DataType(DataType.Date)]
        public string RegistrationTo { get; set; }

        [DisplayName("Package Notes")]
        [DataType(DataType.MultilineText)]
        public string PackageNotes { get; set; }

        [DisplayName("Terms & Conditions")]
        [DataType(DataType.MultilineText)]
        public string TermsConditions { get; set; }

        public bool HaveStayHotel { get; set; } = false;

        public int MinDays { get; set; } = 0;

        public int MaxDays { get; set; } = 0;

        [DisplayName("Event Agendas")]
        public ICollection<EventAgendaModel> EventAgendas { get; set; }

        [DisplayName("Event Packages")]
        public ICollection<EventPackageModel> EventPackages { get; set; }

        [DisplayName("Event Activities")]
        public ICollection<EventActivityModel> EventActivities { get; set; }

        [DisplayName("Event Galleries")]
        public ICollection<EventGalleryModel> EventGalleries { get; set; }
    }
}
