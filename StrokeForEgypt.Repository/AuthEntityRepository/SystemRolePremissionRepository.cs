using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AuthEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.Repository.AuthEntityRepository
{
    public class SystemRolePremissionRepository : AppBaseRepository<SystemRolePremission>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public SystemRolePremissionRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public async Task<List<SystemRolePremission>> GetAllInclude(Expression<Func<SystemRolePremission, bool>> expression)
        {
            return await DBContext.SystemRolePremission.Where(expression)
                .Include(a => a.SystemRole)
                .Include(a => a.SystemView)
                .Include(a => a.AccessLevel)
                .ToListAsync();
        }

        public SystemRole CreateEntity(SystemRole SystemRole, List<int> Views, AccessLevelEnum AccessLevel)
        {
            if (SystemRole.SystemRolePremissions != null && Views != null && Views.Any())
            {
                Views.ForEach(Fk_SystemView => SystemRole.SystemRolePremissions.Add(new SystemRolePremission
                {
                    Fk_AccessLevel = (int)AccessLevel,
                    Fk_SystemView = Fk_SystemView
                }));
            }

            return SystemRole;
        }

        public SystemRole DeleteEntity(SystemRole SystemRole, List<int> Views)
        {
            if (SystemRole.SystemRolePremissions != null && Views != null && Views.Any())
            {
                Views.ForEach(Fk_SystemView => SystemRole.SystemRolePremissions.Remove(SystemRole.SystemRolePremissions.First(a => a.Fk_SystemView == Fk_SystemView)));
            }
            return SystemRole;
        }

        public SystemRole UpdateEntity(SystemRole SystemRole, List<int> OldViews, List<int> NewViews, AccessLevelEnum AccessLevel)
        {
            if (OldViews != null && NewViews != null && SystemRole.SystemRolePremissions != null)
            {
                List<int> AddViews = NewViews.Except(OldViews).ToList();
                List<int> RmvViews = OldViews.Except(NewViews).ToList();
                SystemRole = CreateEntity(SystemRole, AddViews, AccessLevel);
                SystemRole = DeleteEntity(SystemRole, RmvViews);
            }
            return SystemRole;
        }


    }
}
