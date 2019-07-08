using AutoMapper;
using Chess.BusinessLogic.Interfaces;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.DTOs;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using System.Linq;
using System;
using System.Threading.Tasks;
using Chess.DataAccess.Helpers;
using Chess.DataAccess.ElasticSearch;
using Chess.DataAccess.ElasticSearch.Interfaces;
using Chess.DataAccess.ElasticSearch.Models;

namespace Chess.BusinessLogic.Services
{
    public class UserService : CRUDService<User, UserDTO>, IUserService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISignalRNotificationService _notificationService;
        private readonly ISearchService _searchService;
        public UserService(
            IMapper mapper,
            IUnitOfWork uow,
            ICurrentUserProvider currentUserProvider,
            ISignalRNotificationService notificationService,
            ISearchService searchService
            ) : base(mapper, uow)
        {
            _currentUserProvider = currentUserProvider;
            _notificationService = notificationService;
            _searchService = searchService;
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

        [Obsolete("Backend no longer serves online status.Currently firebase realtime database serves it.")]
        public async Task<PagedResultDTO<UserDTO>> GetOnlineUsers(int? pageIndex, int? pageSize)
        {
            if (uow == null)
                return null;

            var onlineHubUsers = _notificationService.GetOnlineUsersInfo();
            var onlineUserPagedProfiles = await uow.GetRepository<User>()
                .GetAllPagedAsync(pageIndex, pageSize, u => onlineHubUsers.Keys.Contains(u.Uid));
            if (onlineUserPagedProfiles == null)
                return null;

            return mapper.Map<PagedResultDTO<UserDTO>>(onlineUserPagedProfiles);
        }

        public async Task<PagedResultDTO<UserDTO>> SearchUsers(string query, bool isOnline, int? pageIndex, int? pageSize)
        {
            if (uow == null)
                return null;

            PagedResult<UserIndex> usersPagedProfiles = null;
            var onlineHubUsers = _notificationService.GetOnlineUsersInfo();
            if (string.IsNullOrWhiteSpace(query))
            {
                usersPagedProfiles = await _searchService.SearchUsers(query, pageSize, pageIndex);
            } else
            {
                query = query.Trim().ToLower();
                if (isOnline)
                {
#warning change logic!
                    onlineHubUsers = _notificationService.GetOnlineUsersInfoByNameOrSurnameStartsWith(query);
                    //usersPagedProfiles = await uow.GetRepository<User>()
                    //    .GetAllPagedAsync(pageIndex, pageSize, u => onlineHubUsers.Keys.Contains(u.Uid));
                }
                else
                {
                    usersPagedProfiles = await _searchService.SearchUsers(query, pageSize, pageIndex);
                    //await uow.GetRepository<User>()
                    //.GetAllPagedAsync(pageIndex, pageSize, u => u.Name.ToLower().StartsWith(query) || u.Name.ToLower().Contains(partForNextWord));
                }
            }
            
            if (usersPagedProfiles == null)
                return null;

            var result = mapper.Map<PagedResult<UserIndex>, PagedResultDTO<UserDTO>>(usersPagedProfiles, opt => opt.AfterMap((src, dest) =>
            {
                //var srcUsers = src.DataRows;
                //foreach (var user in dest.DataRows)
                //{
                //    user.IsOnline = (onlineHubUsers.ContainsKey(srcUsers.First(u => u.Id == user.Id).Uid)) ? true : false;
                //}
            }));
            return result;
        }

        public async Task<string> ReIndex()
        {
            var users = await uow.GetRepository<User>().GetAllAsync();
            var res = await ESRepository.ReIndex(users);

            return res;
        }

    }
}
