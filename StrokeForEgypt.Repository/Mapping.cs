using AutoMapper;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Service.AccountEntity;
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

                cfg.AllowNullCollections = true;
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
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            #endregion

            #region EventEntity

            CreateMap<Event, EventModel>()
                .ForMember(dest => dest.EventAgendas, opt => opt.Ignore())
                .ForMember(dest => dest.EventPackages, opt => opt.Ignore())
                .ForMember(dest => dest.EventActivities, opt => opt.Ignore())
                .ForMember(dest => dest.EventGalleries, opt => opt.Ignore());

            CreateMap<EventActivity, EventActivityModel>();

            CreateMap<EventAgenda, EventAgendaModel>()
                .ForMember(dest => dest.EventAgendaGalleries, opt => opt.Ignore());

            CreateMap<EventAgendaGallery, EventAgendaGalleryModel>();

            CreateMap<EventGallery, EventGalleryModel>();

            CreateMap<EventPackage, EventPackageModel>();


            #endregion

            #region MainDataEntity

            CreateMap<AppAbout, AppAboutModel>();

            CreateMap<City, CityModel>();

            CreateMap<Country, CountryModel>();

            CreateMap<Gallery, GalleryModel>();

            CreateMap<Gender, GenderModel>();

            #endregion

            #region NewsEntity

            CreateMap<News, NewsModel>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.NewsGalleries, opt => opt.Ignore());

            CreateMap<NewsGallery, NewsGalleryModel>();

            #endregion

            #region NotificationEntity

            CreateMap<Notification, NotificationModel>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.OpenType, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationType, opt => opt.Ignore());

            CreateMap<NotificationType, NotificationTypeModel>();

            CreateMap<OpenType, OpenTypeModel>();

            #endregion

            #region AccountEntity

            CreateMap<RegisterModel, Account>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(a => a.Password))
                .ForMember(dest => dest.LoginTokenHash, opt => opt.MapFrom(a => a.LoginToken));

            CreateMap<Account, AccountFullModel>();

            CreateMap<EditProfileModel, Account>();

            CreateMap<AccountDeviceModel, AccountDevice>();

            #endregion

        }
    }

    public class DateTimeValueConverter : IValueConverter<DateTime, string>
    {
        public string Convert(DateTime sourceMember, ResolutionContext context)
        {
            return sourceMember.ToString("dd/M/yyyy hh:mm tt", CultureInfo.InvariantCulture);
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
