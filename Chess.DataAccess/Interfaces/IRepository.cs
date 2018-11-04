using Chess.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chess.DataAccess.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity, new()
    {
        Task<TEntity> AddAsync(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Remove(TEntity entity);

        Task<TEntity> RemoveByIdAsync(int id);

        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
