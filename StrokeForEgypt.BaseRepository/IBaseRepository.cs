using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StrokeForEgypt.BaseRepository
{
    public interface IBaseRepository<T>
    {
        Task<List<T>> GetAll();

        Task<List<T>> GetAll(Expression<Func<T, bool>> expression);

        Task<T> GetByID(int id);

        bool Any();

        bool Any(Expression<Func<T, bool>> expression);

        T CreateEntity(T entity);

        void CreateEntity(List<T> entities);

        void UpdateEntity(T entity);

        void UpdateEntity(List<T> entities);

        void DeleteEntity(List<T> entities);

        Task<int> Save();
    }
}
