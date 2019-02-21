using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.DTOs;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.DataAccess.Helpers;

namespace Chess.BusinessLogic.Services
{
    public class UserService : CRUDService<User, UserDTO>, IUserService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISignalRNotificationService _notificationService;
        public UserService(
            IMapper mapper,
            IUnitOfWork uow,
            ICurrentUserProvider currentUserProvider,
            ISignalRNotificationService notificationService
            ) : base(mapper, uow)
        {
            _currentUserProvider = currentUserProvider;
            _notificationService = notificationService;
        }

        public async Task<UserDTO> GetCurrentUser()
        {
            if (uow == null)
                return null;

            var currentUser =  await _currentUserProvider.GetCurrentUserAsync();
            if (currentUser == null)
                return null;

            var currentDbUser = await uow.GetRepository<User>()
                .GetOneAsync(u => string.Equals(u.Uid, currentUser.Uid));
            var currentUserDTO = mapper.Map<UserDTO>(currentUser);
            currentUserDTO.Uid = currentUser.Uid;
            currentUserDTO.Id = currentDbUser.Id;
            return currentUserDTO;
        }

        public async Task<PagedResultDTO<UserDTO>> GetOnlineUsers(int? pageIndex, int? pageSize)
        {
            if (uow == null)
                return null;

            var onlineHubUsers = _notificationService.GetOnlineUsersInfo();
            var onlineUserPagedProfiles = await uow.GetRepository<User>()
                .GetAllAsync(pageIndex, pageSize, u => onlineHubUsers.Keys.Contains(u.Uid));
            if (onlineUserPagedProfiles == null)
                return null;

            return mapper.Map<PagedResultDTO<UserDTO>>(onlineUserPagedProfiles);
        }

        public async Task<PagedResultDTO<UserDTO>> GetUsersByNameOrSurnameStartsWith(string part, bool isOnline, int? pageIndex, int? pageSize)
        {
            if (uow == null)
                return null;

            PagedResult<User> usersPagedProfiles = null;
            if(string.IsNullOrWhiteSpace(part))
            {
                usersPagedProfiles = await uow.GetRepository<User>()
                    .GetAllAsync(pageIndex, pageSize);
            } else
            {
                part = part.Trim().ToLower();
                if (isOnline)
                {
                    var onlineHubUsers = _notificationService.GetOnlineUsersInfoByNameOrSurnameStartsWith(part);
                    usersPagedProfiles = await uow.GetRepository<User>()
                        .GetAllAsync(pageIndex, pageSize, u => onlineHubUsers.Keys.Contains(u.Uid));
                }
                else
                {
                    var partForNextWord = $" {part}";
                    usersPagedProfiles = await uow.GetRepository<User>()
                        .GetAllAsync(pageIndex, pageSize, u => u.Name.ToLower().StartsWith(part) || u.Name.ToLower().Contains(partForNextWord));
                }
            }

            if (usersPagedProfiles == null)
                return null;

            return mapper.Map<PagedResultDTO<UserDTO>>(usersPagedProfiles);
        }
    }
}
