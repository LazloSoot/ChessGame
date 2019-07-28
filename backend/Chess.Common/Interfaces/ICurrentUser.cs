using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<User> GetCurrentUserAsync();
        Task<User> GetCurrentDbUserAsync();
        string GetCurrentUserUid();
    }
}
