using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using Elasticsearch.Net;
using Chess.DataAccess.ElasticSearch.Interfaces;
using Chess.DataAccess.Helpers;
using Chess.DataAccess.Entities;
using System.Threading.Tasks;
using Chess.DataAccess.ElasticSearch.Models;
using System.Collections.Generic;
using System.Linq;
using Chess.DataAccess.ElasticSearch.Services;

namespace Chess.DataAccess.ElasticSearch
{
    public static class ESRepository
    {
        private static IElasticLowLevelClient _lowlevelClient;

        private static ConnectionSettings _settings;

        public static bool IsElasticUsed { get; private set; }
        public static IElasticClient Client { get; private set; }

        public static void AddElasticSearch(this IServiceCollection services, IConfiguration config)
        {
            var url = config["elasticsearch:url"];
            var defaultIndex = config["elasticsearch:index"];
            IsElasticUsed = bool.Parse(config["elasticsearch:updateIndex"]);
            _settings = new ConnectionSettings(new Uri(url))
                .DefaultFieldNameInferrer(s => s)
                .DefaultMappingFor<UserIndex>(m => m
                    .IndexName("user")
                    .TypeName("user")
                );
            _settings.EnableDebugMode();

            if (!string.IsNullOrWhiteSpace(defaultIndex))
            {
                _settings.DefaultIndex(defaultIndex);
            }

            Client = new ElasticClient(_settings);
            services.AddSingleton<IElasticClient>(Client);
            services.AddTransient<ISearchService>(a => new SearchService(Client));

            var settingslow = new ConnectionConfiguration(new Uri(url))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            _lowlevelClient = new ElasticLowLevelClient(settingslow);
        }

        public static async Task UpdateSearchIndex<T>(T entityToUpdate, CRUDAction action) where T : Entity, new()
        {
            if (IsElasticUsed)
            {
                if (entityToUpdate is IElasticSearcheable searchable)
                {
                    var indexObject = searchable.GetIndexObject();
                    if (indexObject != null)
                    {
                        var targetType = typeof(T).Name.ToLower();
                        try
                        {
                            StringResponse response;
                            switch (action)
                            {
                                case CRUDAction.Create:
                                    {
                                        indexObject.CreatedAt = DateTime.Now;
                                        response = await _lowlevelClient.IndexAsync<StringResponse>(targetType, targetType,
                                            indexObject.Id, PostData.Serializable(indexObject));
                                    }
                                    break;
                                case CRUDAction.Update:
                                    {
                                        indexObject.UpdatedAt = DateTime.Now;
                                        response =
                                        await _lowlevelClient.IndexPutAsync<StringResponse>(targetType, targetType,
                                            indexObject.Id, PostData.Serializable(indexObject));
                                        break;
                                    }
                                case CRUDAction.Delete:
                                    {
                                        response =
                                            await _lowlevelClient.DeleteAsync<StringResponse>(targetType, targetType,
                                                indexObject.Id);
                                        break;
                                    }
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }

        public static async Task<string> ReIndex<T>(IEnumerable<T> data) where T : Entity, IElasticSearcheable, new()
        {
            var targetType = typeof(T).Name.ToLower();
            await _lowlevelClient.IndicesDeleteAsync<StringResponse>(targetType);

            foreach (var post in data)
            {
                var indexObject = post.GetIndexObject();
                indexObject.UpdatedAt = DateTime.Now;
                await _lowlevelClient.IndexAsync<StringResponse>(targetType, targetType,
                    indexObject.Id, PostData.Serializable(indexObject));
            }

            return $"{data.Count()} reindexed";
        }
    }
}
