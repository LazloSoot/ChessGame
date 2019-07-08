using Chess.DataAccess.Entities;
using Chess.DataAccess.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chess.DataAccess.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity, new()
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<TEntity> RemoveAsync(TEntity entity);

        Task<TEntity> RemoveByIdAsync(int id);

        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null);
        
        Task<PagedResult<TEntity>> GetAllPagedAsync(int? pageIndex = null, int? pageSize = null, Expression < Func<TEntity, bool>> predicate = null);
    }
}
