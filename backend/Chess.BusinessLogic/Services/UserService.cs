using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.Common.DTOs;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Services
{
    public class UserService : CRUDService<User, UserDTO>, IUserService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        public UserService(
            IMapper mapper,
            IUnitOfWork uow,
            ICurrentUserProvider currentUserProvider
            ) : base(mapper, uow)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<UserDTO> GetCurrentUser()
        {
            var currentUser =  await _currentUserProvider.GetCurrentUserAsync();
            if (currentUser == null)
                return null;

            var currentUserDTO = mapper.Map<UserDTO>(currentUser);
            currentUserDTO.Uid = currentUser.Uid;
            return currentUserDTO;
        }
    }
}
