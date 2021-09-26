using AutoMapper;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Entity.SponsorEntity;
using StrokeForEgypt.Service.AccountEntity;
using StrokeForEgypt.Service.BookingEntity;
using StrokeForEgypt.Service.EventEntity;
using StrokeForEgypt.Service.MainDataEntity;
using StrokeForEgypt.Service.NewsEntity;
using StrokeForEgypt.Service.NotificationEntity;
using System;
using System.Globalization;

namespace StrokeForEgypt.Repository
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            MapperConfiguration configuration = new(cfg =>
            {
                cfg.CreateMap<DateTime, string>().ConvertUsing(new DateTimeTypeConverter());
                cfg.CreateMap<DateTime, string>().ConvertUsing(new DateTimeTypeConverter());

                cfg.AllowNullCollections = false;
            });

            #region AuthEntity
            CreateMap<SystemView, SystemView>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SystemRolePremissions, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<SystemRole, SystemRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SystemRolePremissions, opt => opt.Ignore())
                .ForMember(dest => dest.SystemUsers, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<SystemUser, SystemUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SystemRole, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            #endregion

            #region EventEntity

            CreateMap<Event, EventModel>()
                .ForMember(dest => dest.FromDate, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.ToDate, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.RegistrationFrom, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.RegistrationTo, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.LongDescription, opt => opt.ConvertUsing(new StringConverter()))
                .ForMember(dest => dest.PackageNotes, opt => opt.ConvertUsing(new StringConverter()))
                .ForMember(dest => dest.HotelName, opt => opt.ConvertUsing(new StringConverter()))
                .ForMember(dest => dest.TermsConditions, opt => opt.ConvertUsing(new StringConverter()))
                .ForMember(dest => dest.EventAgendas, opt => opt.Ignore())
                .ForMember(dest => dest.EventPackages, opt => opt.Ignore())
                .ForMember(dest => dest.EventActivities, opt => opt.Ignore())
                .ForMember(dest => dest.EventGalleries, opt => opt.Ignore());

            CreateMap<EventActivity, EventActivityModel>();

            CreateMap<EventAgenda, EventAgendaModel>()
                .ForMember(dest => dest.FromDate, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.FromTime, opt => opt.ConvertUsing(new TimeValueConverter()))
                .ForMember(dest => dest.ToDate, opt => opt.ConvertUsing(new DateValueConverter()))
                .ForMember(dest => dest.ToTime, opt => opt.ConvertUsing(new TimeValueConverter()))
                .ForMember(dest => dest.LongDescription, opt => opt.ConvertUsing(new StringConverter()))

                .ForMember(dest => dest.EventAgendaGalleries, opt => opt.Ignore());

            CreateMap<EventAgendaGallery, EventAgendaGalleryModel>();

            CreateMap<EventGallery, EventGalleryModel>();

            CreateMap<EventPackage, EventPackageModel>()
                .ForMember(dest => dest.Event, opt => opt.Ignore());

            CreateMap<Event, Event>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
                .ForMember(dest => dest.EventAgendas, opt => opt.Ignore())
                .ForMember(dest => dest.EventActivities, opt => opt.Ignore())
                .ForMember(dest => dest.EventPackages, opt => opt.Ignore())
                .ForMember(dest => dest.EventGalleries, opt => opt.Ignore())
                .ForMember(dest => dest.News, opt => opt.Ignore())
                .ForMember(dest => dest.Sponsors, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<EventActivity, EventActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMemberActivities, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Fk_Event, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<EventAgenda, EventAgenda>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.EventAgendaGalleries, opt => opt.Ignore())
               .ForMember(dest => dest.Event, opt => opt.Ignore())
               .ForMember(dest => dest.Fk_Event, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<EventPackage, EventPackage>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
               .ForMember(dest => dest.Bookings, opt => opt.Ignore())
               .ForMember(dest => dest.Event, opt => opt.Ignore())
               .ForMember(dest => dest.Fk_Event, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            #endregion

            #region MainDataEntity

            CreateMap<AppAbout, AppAboutModel>()
                .ForMember(dest => dest.DownMessage, opt => opt.ConvertUsing(new StringConverter()))
                .ForMember(dest => dest.Description, opt => opt.ConvertUsing(new StringConverter()));

            CreateMap<City, CityModel>();

            CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.Cities, opt => opt.Ignore());

            CreateMap<Gallery, GalleryModel>();

            CreateMap<Gender, GenderModel>();

            CreateMap<AppAbout, AppAbout>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<City, City>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMembers, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Country, Country>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cities, opt => opt.Ignore())
                .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            #endregion

            #region NewsEntity

            CreateMap<News, NewsModel>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.NewsGalleries, opt => opt.Ignore());

            CreateMap<NewsGallery, NewsGalleryModel>();


            CreateMap<News, News>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.NewsGalleries, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            #endregion

            #region NotificationEntity

            CreateMap<Notification, NotificationModel>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationType, opt => opt.Ignore());

            CreateMap<NotificationType, NotificationTypeModel>();


            CreateMap<Notification, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationAccounts, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationType, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<NotificationType, NotificationType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            #endregion

            #region AccountEntity

            CreateMap<Account, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.LoginTokenHash, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationCodeHash, opt => opt.Ignore())
                .ForMember(dest => dest.AccountDevices, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationAccounts, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

            CreateMap<RegisterModel, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(a => a.Password))
                .ForMember(dest => dest.LoginTokenHash, opt => opt.MapFrom(a => a.LoginToken));

            CreateMap<Account, AccountFullModel>();

            CreateMap<EditProfileModel, Account>();

            CreateMap<AccountDeviceModel, AccountDevice>();

            #endregion

            #region BookingEntity

            CreateMap<BookingCreateModel, Booking>();


            CreateMap<Booking, BookingModel>()
                .ForMember(dest => dest.EventPackage, opt => opt.Ignore())
                .ForMember(dest => dest.BookingState, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Booking, Booking>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore())
                .ForMember(dest => dest.Fk_Account, opt => opt.Ignore())
                .ForMember(dest => dest.EventPackage, opt => opt.Ignore())
                .ForMember(dest => dest.BookingState, opt => opt.Ignore())
                .ForMember(dest => dest.BookingStateHistories, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMembers, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<BookingMemberCreateModel, BookingMember>();

            CreateMap<BookingMember, BookingMember>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Booking, opt => opt.Ignore())
                .ForMember(dest => dest.Fk_Booking, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMemberActivities, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMemberAttachments, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMemberActivities, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<BookingMember, BookingMemberModel>()
                .ForMember(dest => dest.BookingMemberAttachments, opt => opt.Ignore())
                .ForMember(dest => dest.BookingMemberActivities, opt => opt.Ignore());

            CreateMap<BookingMemberAttachment, BookingMemberAttachmentModel>()
                .ForMember(dest => dest.BookingMember, opt => opt.Ignore());

            CreateMap<BookingState, BookingState>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BookingStateHistories, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<BookingState, BookingStateModel>();
            #endregion

            #region SponsorEntity
            CreateMap<Sponsor, Sponsor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SponsorType, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<SponsorType, SponsorType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Sponsors, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            #endregion

        }
    }

    public class DateValueConverter : IValueConverter<DateTime, string>
    {
        public string Convert(DateTime sourceMember, ResolutionContext context)
        {
            return sourceMember.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
        }
    }

    public class TimeValueConverter : IValueConverter<TimeSpan, string>
    {
        public string Convert(TimeSpan source, ResolutionContext context)
        {
            DateTime dateTime = DateTime.Now.Date + source;
            return dateTime.ToString("h:mm tt");
        }
    }

    public class DateTimeTypeConverter : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return source.ToString("dd/M/yyyy hh:mm tt", CultureInfo.InvariantCulture);
        }
    }

    public class StringConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
            {
                sourceMember = sourceMember.Replace("font-family", "font-f");
            }
            return string.IsNullOrEmpty(sourceMember) ? "" : sourceMember;
        }
    }

    public class FileTypeConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember == "application/octet-stream" ? "image/jpeg" : sourceMember;
        }
    }
}
