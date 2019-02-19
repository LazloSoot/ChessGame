using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Chess.DataAccess.Helpers;

namespace Chess.DataAccess.SqlRepositories
{
    public class ChessRepository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> dbSet;

        public ChessRepository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await dbSet.AddAsync(entity)).Entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public TEntity Update(TEntity entity)
        {
            return dbSet.Update(entity).Entity;
        }

        public async Task<PagedResult<TEntity>> GetAllAsync(int? pageIndex = null, int? pageSize = null, Expression<Func<TEntity, bool>> predicate = null)
        {
            var resultPage = new PagedResult<TEntity>();
            resultPage.PageIndex = (pageIndex.HasValue && pageIndex.Value >= 0) ? pageIndex.Value : 0;
            resultPage.PageSize = (pageSize.HasValue && pageSize.Value > 0) ? pageSize.Value : int.MaxValue;

            var query = (predicate != null) ? dbSet.Where(predicate) : dbSet;
            resultPage.TotalDataRowsCount = query.Count();
            resultPage.PageCount = (int)(Math.Ceiling((double)resultPage.TotalDataRowsCount / resultPage.PageSize));
            resultPage.DataRows = await query
                .Skip(pageSize.Value * pageIndex.Value)
                .Take(pageSize.Value)
                .ToListAsync();

            return resultPage;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.Where(predicate).FirstOrDefaultAsync();
        }

        public TEntity Remove(TEntity entity)
        {
            return dbSet.Remove(entity).Entity;
        }

        public async Task<TEntity> RemoveByIdAsync(int id)
        {
            var target = await dbSet.FindAsync(id);
            if(target != null)
            {
                target = dbSet.Remove(target).Entity;
            }

            return target;
        }
    }
}
