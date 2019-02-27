using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Common.DTOs
{
    public class PagedResultDTO<T>
        where T : DbEntityDTO, new()
    {
        public int PageIndex { get; set; }
        public long PageCount { get; set; }
        public int PageSize { get; set; }
        public long TotalDataRowsCount { get; set; }
        public IEnumerable<T> DataRows { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public PagedResultDTO()
        {
            DataRows = new List<T>();
        }
    }
}
