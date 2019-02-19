using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Common.DTOs
{
    public class PagedResultDTO<T>
        where T : DbEntityDTO, new()
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalDataRowsCount { get; set; }
        public IEnumerable<T> DataRows { get; set; }
        public PagedResultDTO()
        {
            DataRows = new List<T>();
        }
    }
}
