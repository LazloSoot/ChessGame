using System;
namespace Chess.DataAccess.ElasticSearch.Interfaces
{
    public interface IIndexObject
    {
        /// <summary>
        /// The mandatory Key, normally Guid
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Updated at time
        /// </summary>
        DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Created at time
        /// </summary>
        DateTime CreatedAt { get; set; }

    }
}
