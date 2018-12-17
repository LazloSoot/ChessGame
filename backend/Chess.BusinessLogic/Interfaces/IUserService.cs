using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IUserService : ICRUDService<User, UserDTO>
    {
        Task<UserDTO> GetByUid(string uid);
    }
}
