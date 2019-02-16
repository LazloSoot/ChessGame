using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;

namespace Chess.BusinessLogic.Interfaces
{
    public interface ICRUDService<TEntity, TEntityDTO> 
        where TEntity : Entity, new()
        where TEntityDTO : DbEntityDTO, new() 
    {
        Task<TEntityDTO> GetByIdAsync(int id);

        Task<TEntityDTO> UpdateAsync(TEntityDTO entity);

        Task<PagedResultDTO<TEntityDTO>> GetListAsync(int? pageIndex = null, int? pageSize = null);

        Task<TEntityDTO> AddAsync(TEntityDTO entity);

        Task<bool> TryRemoveAsync(int id);
    }
}
