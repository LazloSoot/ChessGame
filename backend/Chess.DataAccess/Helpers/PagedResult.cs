using Chess.DataAccess.Entities;
using System.Collections.Generic;

namespace Chess.DataAccess.Helpers
{
    public class PagedResult<T> 
        where T: Entity, new()
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalDataRowsCount { get; set; }
        public IEnumerable<T> DataRows { get; set; }
        public PagedResult()
        {
            DataRows = new List<T>();
        }
    }
}
