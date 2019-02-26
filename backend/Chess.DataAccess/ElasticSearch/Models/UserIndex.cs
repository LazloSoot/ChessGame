using Chess.DataAccess.ElasticSearch.Interfaces;
using System;
using Nest;

namespace Chess.DataAccess.ElasticSearch.Models
{
    public class UserIndex : IIndexObject
    {
        public string Id { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        [Completion]
        public string Name { get; set; }
    }
}
