using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IUserService : ICRUDService<User, UserDTO>
    {
        Task<UserDTO> GetCurrentUser();
        //Task<UserDTO> GetByUid(string uid);
        Task<PagedResultDTO<UserDTO>> GetOnlineUsers(int? pageIndex, int? pageSize);
        Task<PagedResultDTO<UserDTO>> GetOnlineUsersByNameOrSurnameStartsWith(string part, int? pageIndex, int? pageSize);
    }
}
