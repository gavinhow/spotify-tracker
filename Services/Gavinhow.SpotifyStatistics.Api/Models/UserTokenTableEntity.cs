using System;
using Azure;
using Azure.Data.Tables;

namespace Gavinhow.SpotifyStatistics.Api.Models
{
    /// <summary>
    /// POCO mapped to the "Users" Azure Table Storage table.
    /// Azure.Data.Tables serialises/deserialises any class that exposes
    /// PartitionKey, RowKey, ETag and Timestamp — no base-class required.
    /// </summary>
    public class UserTokenTableEntity: ITableEntity
    {
        public UserTokenTableEntity() { }

        public UserTokenTableEntity(string userId)
        {
            PartitionKey = userId;
            RowKey = userId;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string SpotifyAccessToken { get; set; }
        public string SpotifyRefreshToken { get; set; }
        public DateTimeOffset? TokenExpiry { get; set; }
        public DateTimeOffset? LastSynced { get; set; }
        public bool IsDisabled { get; set; }

        // Populated by the SDK on read; required for optimistic-concurrency updates
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
