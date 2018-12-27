using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.Common.Interfaces
{
    public interface ICurrentUser
    {
        Task<User> GetCurrentUserAsync();
    }
}
