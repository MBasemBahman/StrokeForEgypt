using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Entity.SponsorEntity;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.DAL
{
    public class BaseDBContext : DbContext
    {
        public BaseDBContext(DbContextOptions<BaseDBContext> options) : base(options)
        {
        }

        #region AccountEntity

        public DbSet<Account> Account { get; set; }
        public DbSet<AccountDevice> AccountDevice { get; set; }

        #endregion

        #region AuthEntity

        public DbSet<AccessLevel> AccessLevel { get; set; }
        public DbSet<SystemRole> SystemRole { get; set; }
        public DbSet<SystemRolePremission> SystemRolePremission { get; set; }
        public DbSet<SystemUser> SystemUser { get; set; }
        public DbSet<SystemView> SystemView { get; set; }

        #endregion

        #region BookingEntity

        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingMember> BookingMember { get; set; }
        public DbSet<BookingMemberActivity> BookingMemberActivity { get; set; }
        public DbSet<BookingMemberAttachment> BookingMemberAttachment { get; set; }
        public DbSet<BookingState> BookingState { get; set; }
        public DbSet<BookingStateHistory> BookingStateHistory { get; set; }

        #endregion

        #region EventEntity

        public DbSet<Event> Event { get; set; }
        public DbSet<EventActivity> EventActivity { get; set; }
        public DbSet<EventAgenda> EventAgenda { get; set; }
        public DbSet<EventAgendaGallery> EventAgendaGallery { get; set; }
        public DbSet<EventGallery> EventGallery { get; set; }
        public DbSet<EventPackage> EventPackage { get; set; }

        #endregion

        #region MainDataEntity

        public DbSet<AppAbout> AppAbout { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Gender> Gender { get; set; }

        #endregion

        #region NewsEntity

        public DbSet<News> News { get; set; }
        public DbSet<NewsGallery> NewsGallery { get; set; }

        #endregion

        #region NotificationEntity

        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationAccount> NotificationAccount { get; set; }
        public DbSet<NotificationType> NotificationType { get; set; }
        public DbSet<OpenType> OpenType { get; set; }

        #endregion

        #region SponsorEntity

        public DbSet<Sponsor> Sponsor { get; set; }
        public DbSet<SponsorType> SponsorType { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region AuthEntity

            #region AccessLevel

            modelBuilder.Entity<AccessLevel>()
           .HasData(
               new AccessLevel
               {
                   Id = (int)AccessLevelEnum.ReadAccess,
                   Name = AccessLevelEnum.ReadAccess.ToString()
               },
               new AccessLevel
               {
                   Id = (int)AccessLevelEnum.CreateOrUpdateAccess,
                   Name = AccessLevelEnum.CreateOrUpdateAccess.ToString()
               },
               new AccessLevel
               {
                   Id = (int)AccessLevelEnum.FullAccess,
                   Name = AccessLevelEnum.FullAccess.ToString()
               }
           );

            #endregion

            #region SystemRole
            modelBuilder.Entity<SystemRole>()
             .HasData(
                 new SystemRole
                 {
                     Id = (int)SystemRoleEnum.Developer,
                     Name = SystemRoleEnum.Developer.ToString(),
                 }

             );
            #endregion

            #region SystemUser
            modelBuilder.Entity<SystemUser>()
           .HasData(
               new SystemUser
               {
                   Id = (int)SystemUserEnum.Developer,
                   Phone = "01069946657",
                   FullName = SystemUserEnum.Developer.ToString(),
                   JobTitle = SystemUserEnum.Developer.ToString(),
                   Email = "Developer@mail.com",
                   Password = RandomGenerator.RandomKey(),
                   IsActive = true,
                   Fk_SystemRole = (int)SystemRoleEnum.Developer
               }
           );
            #endregion

            #region SystemView
            modelBuilder.Entity<SystemView>()
           .HasData(
               new SystemView
               {
                   Id = (int)SystemViewEnum.Home,
                   Name = "Home",
                   DisplayName = "Home Page"
               },
               new SystemView
               {
                   Id = (int)SystemViewEnum.SystemUser,
                   Name = "SystemUser",
                   DisplayName = "System Users"
               },
               new SystemView
               {
                   Id = (int)SystemViewEnum.SystemView,
                   Name = "SystemView",
                   DisplayName = "System Views"
               },
                new SystemView
                {
                    Id = (int)SystemViewEnum.SystemRole,
                    Name = "SystemRole",
                    DisplayName = "System Roles"
                }
           );
            #endregion

            #region SystemRolePermission
            modelBuilder.Entity<SystemRolePremission>()
             .HasData(
                 new SystemRolePremission
                 {
                     Id = 1,
                     Fk_SystemRole = (int)SystemRoleEnum.Developer,
                     Fk_AccessLevel = (int)AccessLevelEnum.FullAccess,
                     Fk_SystemView = (int)SystemViewEnum.Home,
                 },
                 new SystemRolePremission
                 {
                     Id = 2,
                     Fk_SystemRole = (int)SystemRoleEnum.Developer,
                     Fk_AccessLevel = (int)AccessLevelEnum.FullAccess,
                     Fk_SystemView = (int)SystemViewEnum.SystemUser,
                 },
                  new SystemRolePremission
                  {
                      Id = 3,
                      Fk_SystemRole = (int)SystemRoleEnum.Developer,
                      Fk_AccessLevel = (int)AccessLevelEnum.FullAccess,
                      Fk_SystemView = (int)SystemViewEnum.SystemView,
                  },
                    new SystemRolePremission
                    {
                        Id = 4,
                        Fk_SystemRole = (int)SystemRoleEnum.Developer,
                        Fk_AccessLevel = (int)AccessLevelEnum.FullAccess,
                        Fk_SystemView = (int)SystemViewEnum.SystemRole,
                    }
             );
            #endregion

            #endregion

            #region MainDataEntity

            #region Gender

            modelBuilder.Entity<Gender>()
           .HasData(
               new Gender
               {
                   Id = (int)GenderEnum.Male,
                   Name = GenderEnum.Male.ToString()
               },
               new Gender
               {
                   Id = (int)GenderEnum.Female,
                   Name = GenderEnum.Female.ToString()
               }
           );

            #endregion

            #region AppAbout

            modelBuilder.Entity<AppAbout>()
           .HasData(
               new AppAbout
               { }
           );

            #endregion

            #endregion

        }
    }
}
