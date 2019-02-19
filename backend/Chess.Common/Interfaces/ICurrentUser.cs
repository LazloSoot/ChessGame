using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        Task<User> GetCurrentUserAsync();

        string GetCurrentUserUid();
    }
}
