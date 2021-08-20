using AutoMapper;
using StrokeForEgypt.Entity.AuthEntity;
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

            #region AccountEntity

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
