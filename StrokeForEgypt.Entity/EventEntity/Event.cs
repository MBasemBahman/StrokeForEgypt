﻿using StrokeForEgypt.Entity.CommonEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.EventEntity
{
    public class Event : ImageEntity
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
        public DateTime FromDate { get; set; }

        [DisplayName("To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [DisplayName("Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DisplayName("Latitude")]
        public double Latitude { get; set; }

        [DisplayName("Longitude")]
        public double Longitude { get; set; }

        [DisplayName("Registration From")]
        [DataType(DataType.Date)]
        public DateTime RegistrationFrom { get; set; }

        [DisplayName("Registration To")]
        [DataType(DataType.Date)]
        public DateTime RegistrationTo { get; set; }

        [DisplayName("Package Notes")]
        [DataType(DataType.MultilineText)]
        public string PackageNotes { get; set; }

        [DisplayName("Event Agendas")]
        public ICollection<EventAgenda> EventAgendas { get; set; }

        [DisplayName("Event Packages")]
        public ICollection<EventPackage> EventPackages { get; set; }

        [DisplayName("Event Galleries")]
        public ICollection<EventGallery> EventGalleries { get; set; }
    }
}
