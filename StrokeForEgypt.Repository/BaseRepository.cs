using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;

namespace StrokeForEgypt.Repository
{
    public class BaseRepository<T> : AppBaseRepository<T> where T : class
    {
        private readonly BaseDBContext DBContext;

        public BaseRepository(BaseDBContext DBContext) : base(DBContext)
        {
            this.DBContext = DBContext;
        }
    }
}
