using AutoMapper;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.NotificationEntity;
using System.Collections.Generic;
using System.Linq;


namespace StrokeForEgypt.Repository.NotificationEntityRepository
{
   public class NotificationAccountRepository : AppBaseRepository<NotificationAccount>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public NotificationAccountRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public Notification CreateEntity(Notification Notification, List<int> Accounts)
        {
            if (Notification.NotificationAccounts != null && Accounts != null && Accounts.Any())
            {
                Accounts.ForEach(Fk_Account => Notification.NotificationAccounts.Add(new NotificationAccount
                {
                    Fk_Account = Fk_Account
                }));
            }

            return Notification;
        }

        public Notification DeleteEntity(Notification Notification, List<int> Accounts)
        {
            if (Notification.NotificationAccounts != null && Accounts != null && Accounts.Any())
            {
                Accounts.ForEach(Fk_Account => Notification.NotificationAccounts.Remove(Notification.NotificationAccounts.First(a => a.Fk_Account == Fk_Account)));
            }
            return Notification;
        }

        public Notification UpdateEntity(Notification Notification, List<int> OldData, List<int> NewData)
        {
            if (OldData != null && NewData != null && Notification.NotificationAccounts != null)
            {
                List<int> AddData = NewData.Except(OldData).ToList();
                List<int> RmvData = OldData.Except(NewData).ToList();
                Notification = CreateEntity(Notification, AddData);
                Notification = DeleteEntity(Notification, RmvData);
            }
            return Notification;
        }
    }
}
