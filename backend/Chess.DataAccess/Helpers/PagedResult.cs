using Chess.DataAccess.Entities;
using System.Collections.Generic;

namespace Chess.DataAccess.Helpers
{
    public class PagedResult<T> 
        where T: new()
    {
        public const int MaxPageSize = 1000;
        public int PageIndex { get; set; }
        public long PageCount { get; set; }
        public int PageSize { get; set; }
        public long TotalDataRowsCount { get; set; }
        public IEnumerable<T> DataRows { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public PagedResult()
        {
            DataRows = new List<T>();
        }
    }
}
