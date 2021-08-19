using AutoMapper;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AuthEntity;
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

        #region AuthEntity

        public BaseRepository<AccessLevel> AccessLevel => new(_DBContext);
        public BaseRepository<SystemRole> SystemRole => new(_DBContext);
        public SystemUserRepository SystemUser => new(_DBContext, _Mapper);
        public SystemRolePremissionRepository SystemRolePremission => new(_DBContext, _Mapper);
        public BaseRepository<SystemView> SystemView => new(_DBContext);

        #endregion
    }
}
