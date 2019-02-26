using Chess.Common.DTOs;
using Chess.DataAccess.ElasticSearch.Models;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Helpers;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IUserService : ICRUDService<User, UserDTO>
    {
        Task<UserDTO> GetCurrentUser();
        //Task<UserDTO> GetByUid(string uid);
        Task<PagedResultDTO<UserDTO>> SearchUsers(string part, bool isOnline, int? pageIndex, int? pageSize);
        Task<PagedResult<UserIndex>> SearchUsers2(string query, bool isOnline, int? pageIndex, int? pageSize);
        Task<PagedResultDTO<UserDTO>> GetOnlineUsers(int? pageIndex, int? pageSize);

        Task<string> ReIndex();
    }
}
