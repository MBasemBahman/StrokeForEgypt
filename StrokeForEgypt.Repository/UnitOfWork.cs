using AutoMapper;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Entity.AuthEntity;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Entity.MainDataEntity;
using StrokeForEgypt.Entity.NewsEntity;
using StrokeForEgypt.Entity.NotificationEntity;
using StrokeForEgypt.Entity.SponsorEntity;
using StrokeForEgypt.Repository.AccountEntityRepository;
using StrokeForEgypt.Repository.AuthEntityRepository;
using System.Threading.Tasks;

namespace StrokeForEgypt.Repository
{
    public class UnitOfWork
    {
        private readonly BaseDBContext _DBContext;
        private readonly IMapper _Mapper;

        public UnitOfWork(BaseDBContext BaseDBContext, IMapper Mapper)
        {
            _DBContext = BaseDBContext;
            _Mapper = Mapper;
        }

        public async Task<int> Save()
        {
            return await _DBContext.SaveChangesAsync();
        }

        #region AccountEntity

        public AccountRepository Account => new(_DBContext, _Mapper);
        public BaseRepository<AccountDevice> AccountDevice => new(_DBContext);
        public BaseRepository<RefreshToken> RefreshToken => new(_DBContext);

        #endregion

        #region AuthEntity

        public BaseRepository<AccessLevel> AccessLevel => new(_DBContext);
        public BaseRepository<SystemRole> SystemRole => new(_DBContext);
        public SystemUserRepository SystemUser => new(_DBContext, _Mapper);
        public SystemRolePremissionRepository SystemRolePremission => new(_DBContext, _Mapper);
        public BaseRepository<SystemView> SystemView => new(_DBContext);

        #endregion

        #region BookingEntity

        public BaseRepository<Booking> Booking => new(_DBContext);
        public BaseRepository<BookingMember> BookingMember => new(_DBContext);
        public BaseRepository<BookingMemberActivity> BookingMemberActivity => new(_DBContext);
        public BaseRepository<BookingMemberAttachment> BookingMemberAttachment => new(_DBContext);
        public BaseRepository<BookingState> BookingState => new(_DBContext);
        public BaseRepository<BookingStateHistory> BookingStateHistory => new(_DBContext);

        #endregion

        #region EventEntity

        public BaseRepository<Event> Event => new(_DBContext);
        public BaseRepository<EventActivity> EventActivity => new(_DBContext);
        public BaseRepository<EventAgenda> EventAgenda => new(_DBContext);
        public BaseRepository<EventAgendaGallery> EventAgendaGallery => new(_DBContext);
        public BaseRepository<EventGallery> EventGallery => new(_DBContext);
        public BaseRepository<EventPackage> EventPackage => new(_DBContext);

        #endregion

        #region MainDataEntity

        public BaseRepository<AppAbout> AppAbout => new(_DBContext);
        public BaseRepository<AppView> AppView => new(_DBContext);
        public BaseRepository<City> City => new(_DBContext);
        public BaseRepository<Country> Country => new(_DBContext);
        public BaseRepository<Gallery> Gallery => new(_DBContext);
        public BaseRepository<Gender> Gender => new(_DBContext);

        #endregion

        #region NewsEntity

        public BaseRepository<News> News => new(_DBContext);
        public BaseRepository<NewsGallery> NewsGallery => new(_DBContext);

        #endregion

        #region NotificationEntity

        public BaseRepository<Notification> Notification => new(_DBContext);
        public BaseRepository<NotificationAccount> NotificationAccount => new(_DBContext);
        public BaseRepository<NotificationType> NotificationType => new(_DBContext);
        public BaseRepository<OpenType> OpenType => new(_DBContext);

        #endregion

        #region SponsorEntity

        public BaseRepository<Sponsor> Sponsor => new(_DBContext);
        public BaseRepository<SponsorType> SponsorType => new(_DBContext);

        #endregion
    }
}
