using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.BookingEntity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StrokeForEgypt.Repository.BookingEntityRepository
{
    public class BookingRepository : AppBaseRepository<Booking>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public BookingRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }
        public async Task<Booking> UpdateStateHistory(Booking Booking, int OldState = 0)
        {
            Booking.BookingStateHistories = await DBContext.BookingStateHistory.Where(a => a.Fk_Booking == Booking.Id).ToListAsync();

            if (Booking.BookingStateHistories == null)
            {
                Booking.BookingStateHistories = new List<BookingStateHistory>();
            }

            if (Booking.Fk_BookingState != OldState)
            {
                Booking.BookingStateHistories.Add(new BookingStateHistory
                {
                    Fk_BookingState = Booking.Fk_BookingState,
                    CreatedBy = !string.IsNullOrEmpty(Booking.LastModifiedBy) ? Booking.LastModifiedBy : Booking.CreatedBy,

                });
            }

            return Booking;
        }

    }
}
