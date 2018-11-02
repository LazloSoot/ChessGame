using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.DataAccess.Entities;

namespace Chess.BusinessLogic.Interfaces
{
    public interface ICRUDService<TEntity, TEntityDTO> 
        where TEntity : Entity, new()
        where TEntityDTO : class, new()
    {
        Task<TEntityDTO> GetByIdAsync(int id);

        Task<IEnumerable<TEntityDTO>> GetListAsync();

        Task<TEntityDTO> AddAsync(TEntityDTO entity);

        Task<bool> TryRemoveAsync(int id);
    }
}
