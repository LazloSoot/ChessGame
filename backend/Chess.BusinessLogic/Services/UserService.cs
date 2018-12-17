using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Services
{
    public class UserService : CRUDService<User, UserDTO>, IUserService
    {
        public UserService(IMapper mapper, IUnitOfWork uow):base(mapper, uow)
        {

        }
        public async Task<UserDTO> GetByUid(string uid)
        {
            if(uow == null)
                return null;

            var user = await uow.GetRepository<User>()
                .GetOneAsync(u => string.Equals(u.Uid, uid));

            if (user == null)
                return null;

            return mapper.Map<UserDTO>(user);
        }
    }
}
