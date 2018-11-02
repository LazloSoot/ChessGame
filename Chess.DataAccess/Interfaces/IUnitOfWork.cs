using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new();

        Task<int> SaveAsync();
    }
}
