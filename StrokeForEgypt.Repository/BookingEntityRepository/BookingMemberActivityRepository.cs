using AutoMapper;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.BookingEntity;
using System.Collections.Generic;
using System.Linq;


namespace StrokeForEgypt.Repository.BookingEntityRepository
{
    public class BookingMemberActivityRepository : AppBaseRepository<BookingMemberActivity>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public BookingMemberActivityRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public BookingMember CreateEntity(BookingMember BookingMember, List<int> Activities)
        {
            if (BookingMember.BookingMemberActivities != null && Activities != null && Activities.Any())
            {
                Activities.ForEach(Fk_Activity => BookingMember.BookingMemberActivities.Add(new BookingMemberActivity
                {
                    Fk_EventActivity = Fk_Activity
                }));
            }

            return BookingMember;
        }

        public BookingMember DeleteEntity(BookingMember BookingMember, List<int> Activities)
        {
            if (BookingMember.BookingMemberActivities != null && Activities != null && Activities.Any())
            {
                Activities.ForEach(Fk_Activity => BookingMember.BookingMemberActivities.Remove(BookingMember.BookingMemberActivities.First(a => a.Fk_EventActivity == Fk_Activity)));
            }
            return BookingMember;
        }

        public BookingMember UpdateEntity(BookingMember BookingMember, List<int> OldData, List<int> NewData)
        {
            if (OldData != null && NewData != null && BookingMember.BookingMemberActivities != null)
            {
                List<int> AddData = NewData.Except(OldData).ToList();
                List<int> RmvData = OldData.Except(NewData).ToList();
                BookingMember = CreateEntity(BookingMember, AddData);
                BookingMember = DeleteEntity(BookingMember, RmvData);
            }
            return BookingMember;
        }
    }
}
