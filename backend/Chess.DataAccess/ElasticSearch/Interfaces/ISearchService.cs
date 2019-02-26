using Chess.DataAccess.Helpers;
using System.Threading.Tasks;

namespace Chess.DataAccess.ElasticSearch.Interfaces
{
    public interface ISearchService
    {
        Task<PagedResult<T>> SearchBy<T>(string query, int? pageSize, int? PageIndex) where T : class, IIndexObject, new();
    }
}
