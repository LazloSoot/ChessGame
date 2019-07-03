using Chess.DataAccess.ElasticSearch.Interfaces;
using Chess.DataAccess.ElasticSearch.Models;
using System;

namespace Chess.DataAccess.Entities
{
    public class User : Entity, IElasticSearcheable
    {
        public string Name { get; set; }

        public string Uid { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime RegistrationDate { get; set; }

        public IIndexObject GetIndexObject()
        {
            return new UserIndex()
            {
                Id = this.Id.ToString(),
                Name = this.Name,
                Uid = Uid
            };
        }
    }
}
