using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            return (await context.AddAsync(entity)).Entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
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
