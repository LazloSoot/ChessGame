using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

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

        public Task<TEntity> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> RemoveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
