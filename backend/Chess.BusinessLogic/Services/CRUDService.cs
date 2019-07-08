using Chess.BusinessLogic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Chess.DataAccess.Interfaces;
using Chess.DataAccess.Entities;
using Chess.Common.DTOs;

namespace Chess.BusinessLogic.Services
{
    public class CRUDService<TEntity, TEntityDTO> : ICRUDService<TEntity, TEntityDTO>
        where TEntity : Entity, new()
        where TEntityDTO : DbEntityDTO, new()
    {
        protected readonly IMapper mapper;
        protected readonly IUnitOfWork uow;

        public CRUDService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.uow = unitOfWork;
        }

        public virtual async Task<TEntityDTO> AddAsync(TEntityDTO entity)
        {
            if (uow == null)
                return null;

            var target = await uow.GetRepository<TEntity>()
                .AddAsync(mapper.Map<TEntityDTO, TEntity>(entity));

            if (target == null)
                return null;

            await uow.SaveAsync();
            return mapper.Map<TEntity, TEntityDTO>(target);
        }

        public virtual async Task<TEntityDTO> UpdateAsync(TEntityDTO entity)
        {
            if (uow == null)
                return null;

            var target = await uow.GetRepository<TEntity>()
                .UpdateAsync(mapper.Map<TEntity>(entity));

            await uow.SaveAsync();
            return mapper.Map<TEntityDTO>(target);
        }

        public virtual async Task<TEntityDTO> GetByIdAsync(int id)
        {
            if (uow == null)
                return null;

            var target = await uow.GetRepository<TEntity>().GetByIdAsync(id);
            return target == null ? null : mapper.Map<TEntityDTO>(target);
        }

        public virtual async Task<PagedResultDTO<TEntityDTO>> GetListAsync(int? pageIndex = null, int? pageSize = null)
        {
            if (uow == null)
                return null;

            var targets = await uow.GetRepository<TEntity>().GetAllPagedAsync(pageIndex, pageSize);
            return mapper.Map<PagedResultDTO<TEntityDTO>>(targets);
        }

        public virtual async Task<bool> TryRemoveAsync(int id)
        {
            if (uow == null)
                return false;

            await uow.GetRepository<TEntity>().RemoveByIdAsync(id);
            return await uow.SaveAsync() > 0;
        }
    }
}
