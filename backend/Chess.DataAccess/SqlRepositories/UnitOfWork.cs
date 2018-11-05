using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chess.DataAccess.SqlRepositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext context;
        private readonly Dictionary<Type, object> repositories;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
            repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new()
        {
            var targetType = typeof(TEntity);
            if (repositories.ContainsKey(targetType))
            {
                return repositories[targetType] as IRepository<TEntity>;
            }
            else
            {
                var repoInstance = new ChessRepository<TEntity>(context);
                repositories.Add(targetType, repoInstance);
                return repoInstance;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
