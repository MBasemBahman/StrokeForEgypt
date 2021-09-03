using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.AuthEntity;
using System.Collections.Generic;
using System.Linq;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.Repository.AuthEntityRepository
{
    public class SystemUserRepository : AppBaseRepository<SystemUser>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public SystemUserRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public bool UserExists(string Email, string Password = null)
        {
            return DBContext.SystemUser
                   .Where(a => a.Email == Email)
                   .Where(a => Password == null ? true : a.Password == Password)
                   .Where(a => a.IsActive == true)
                   .Any();
        }

        public SystemUser GetByEmail(string Email)
        {
            return DBContext.SystemUser.FirstOrDefault(a => a.Email == Email);
        }

        public Dictionary<string, string> GetViews(int Fk_SystemRole)
        {
            return DBContext.SystemRolePremission
                            .Where(a => a.Fk_SystemRole == Fk_SystemRole)
                            .Include(a => a.SystemView)
                            .ToDictionary(a => a.SystemView.Name, a => a.Fk_AccessLevel.ToString());
        }

        public bool CheckAuthorization(int Fk_SystemRole, string ViewName, int Fk_AccessLevel)
        {
            SystemRolePremission premission = DBContext.SystemRolePremission.FirstOrDefault(a => a.Fk_SystemRole == Fk_SystemRole && a.SystemView.Name == ViewName);

            if (premission != null)
            {
                if (premission.Fk_AccessLevel == (int)AccessLevelEnum.FullAccess)
                {
                    return true;
                }
                else if (premission.Fk_AccessLevel == (int)AccessLevelEnum.CreateOrUpdateAccess)
                {
                    if (Fk_AccessLevel == (int)AccessLevelEnum.CreateOrUpdateAccess
                        || Fk_AccessLevel == (int)AccessLevelEnum.ReadAccess)
                    {
                        return true;
                    }
                }
                else if (premission.Fk_AccessLevel == (int)AccessLevelEnum.ReadAccess)
                {
                    if (Fk_AccessLevel == (int)AccessLevelEnum.ReadAccess)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
