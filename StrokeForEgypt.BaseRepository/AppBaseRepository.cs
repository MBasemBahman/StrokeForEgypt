using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace StrokeForEgypt.BaseRepository
{
    public abstract class AppBaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext DbContext;

        public AppBaseRepository(DbContext MedicalAppContext)
        {
            DbContext = MedicalAppContext;
        }

        public async Task<List<T>> GetAll()
        {
            return await DbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await DbContext.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> expression = null, List<string> Includes = null)
        {
            return await GetQuery(expression, Includes).ToListAsync();
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression = null, List<string> Includes = null)
        {
            return await GetQuery(expression, Includes).FirstOrDefaultAsync();
        }

        public async Task<T> GetByID(int id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public T CreateEntity(T entity)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> Entity = DbContext.Set<T>().Add(entity);
            return Entity.Entity;
        }

        public void CreateEntity(List<T> entities)
        {
            DbContext.Set<T>().AddRangeAsync(entities);
        }

        public void UpdateEntity(T entity)
        {
            DbContext.Entry<T>(entity).State = EntityState.Modified;
        }

        public void UpdateEntity(List<T> entities)
        {
            foreach (T entity in entities)
            {
                DbContext.Entry<T>(entity).State = EntityState.Modified;
            }
        }

        public void DeleteEntity(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void DeleteEntity(List<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> Save()
        {
            return await DbContext.SaveChangesAsync();
        }

        public bool Any()
        {
            return DbContext.Set<T>().Any();
        }

        public int Count()
        {
            return DbContext.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Where(expression).Count();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Where(expression).Any();
        }

        public IDictionary<string, string> GetLookUp(Expression<Func<T, bool>> expression = null, string KeyString = "Id", string ValueString = "Name")
        {
            PropertyInfo KeyProperty = typeof(T).GetProperty(KeyString);
            PropertyInfo ValueProperty = typeof(T).GetProperty(ValueString);

            return GetQuery(expression).Select(x => new { Key = KeyProperty.GetValue(x, null), Value = ValueProperty.GetValue(x, null).ToString() })
                                       .ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
        }

        public IQueryable<T> GetQuery(Expression<Func<T, bool>> expression = null, List<string> Includes = null)
        {
            expression ??= (a => true);

            IQueryable<T> Query = DbContext.Set<T>().Where(expression).AsQueryable();

            if (Includes != null)
            {
                foreach (string include in Includes)
                {
                    Query = Query.Include(include);
                }
            }

            return Query;
        }

    }
}
