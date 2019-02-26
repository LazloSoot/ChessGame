﻿using Chess.DataAccess.ElasticSearch.Interfaces;
using Chess.DataAccess.Helpers;
using Nest;
using System;
using System.Threading.Tasks;

namespace Chess.DataAccess.ElasticSearch.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _client;
        public SearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task<PagedResult<T>> SearchBy<T>(string query, int? pageSize, int? PageIndex)
            where T: class, IIndexObject, new()
        {
            var result = new PagedResult<T>()
            {
                PageSize = (pageSize.HasValue && pageSize.Value > 0) ? pageSize.Value : PagedResult<T>.MaxPageSize,
                PageIndex = (PageIndex.HasValue && pageSize.Value >= 0) ? pageSize.Value : 0 
            };

            var responce = await _client.SearchAsync<T>(searchDescriptor => searchDescriptor
                    .Query(queryContainerDescriptor => queryContainerDescriptor
                        .Bool(queryDescriptor => queryDescriptor
                            .Must(queryStringQuery => queryStringQuery
                                .QueryString(queryString => queryString
                                    .Query(query)))))
                                        .From(result.PageSize * result.PageIndex)
                                        .Size(result.PageSize));

            result.TotalDataRowsCount = responce.Total;
            result.ElapsedMilliseconds = responce.Took;
            result.DataRows = responce.Documents;
            result.PageCount = (long)(Math.Ceiling((double)result.TotalDataRowsCount / result.PageSize));
            return result;
        }
    }
}
