using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class FollowRequestRepository : DbRepository<FollowRequest, FollowRequestTableEntity>, IFollowRequestRepository
    {
        private readonly IDbClientFactory _factory;

        public FollowRequestRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName => "FollowRequests";

        public Task<FollowRequest> GetByIdAsync(Guid followRequestId)
        {
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).TableReference;

            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("FollowRequestId", QueryComparisons.Equal, followRequestId));

            var queryResults = table.ExecuteQuery(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Task.FromResult(Map(queryResults.First()));
        }

        public Task<FollowRequest> GetByUserId(Guid receiverId, Guid requesterId)
        {
            return this.RetrieveAsync(receiverId.ToString(), requesterId.ToString());
        }

        public Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId)
        {
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).TableReference;

            // The partition key is the receiver id
            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString())
                    );

            var queryResults = table.ExecuteQuery(tq);
            var mappedResults = queryResults.Select(x => Map(x));

            return Task.FromResult(mappedResults);
        }

        public Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId)
        {
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).TableReference;

            // The row key is the requester id
            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                        TableQuery.GenerateFilterCondition("RequesterId", QueryComparisons.Equal, userId.ToString())
                );

            var queryResults = table.ExecuteQuery(tq);
            var mappedResults = queryResults.Select(x => Map(x));

            return Task.FromResult(mappedResults);
        }

        public async Task<int> GetFollowerCountAsync(Guid userId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString()),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForInt("Status", QueryComparisons.Equal, (int)FollowRequestStatus.Accepted)
                    )
                );
            return await GetEntityCount(tableQuery);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId.ToString()),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForInt("Status", QueryComparisons.Equal, (int)FollowRequestStatus.Accepted)
                    )
                );
            return await GetEntityCount(tableQuery);
        }

        public override FollowRequestTableEntity Map(FollowRequest entity)
        {
            return new FollowRequestTableEntity
            {
                FollowRequestId = entity.FollowRequestId,
                ReceiverId = entity.ReceiverId,
                RequesterId = entity.RequesterId,
                Status = (int)entity.Status,
                TimeCreated = entity.TimeCreated
            };
        }

        public override FollowRequest Map(FollowRequestTableEntity entity)
        {
            return new FollowRequest
            {
                FollowRequestId = entity.FollowRequestId,
                ReceiverId = entity.ReceiverId,
                RequesterId = entity.RequesterId,
                Status = (FollowRequestStatus)entity.Status,
                TimeCreated = entity.TimeCreated
            };
        }

        public override string ResolvePartitionKey(FollowRequestTableEntity entity) => entity.ReceiverId.ToString();

        public override string ResolveRowKey(FollowRequestTableEntity entity) => entity.RequesterId.ToString();

        public async Task<IEnumerable<Guid>> GetFollowerIds(Guid userId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
            .Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString()),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForInt("Status", QueryComparisons.Equal, (int)FollowRequestStatus.Accepted)
                )
            );

            // Select only the partition key from the entity.
            tableQuery = tableQuery.Select(new string[] { "RequesterId" });

            CloudTable cloudTable = _factory.GetClient<FollowRequestTableEntity>(TableName).TableReference;

            EntityResolver<Guid> resolver = (pk, rk, ts, props, etag) => props["RequesterId"].GuidValue.GetValueOrDefault();

            List<Guid> ids = new List<Guid>();

            // Fetch matching entities
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<Guid> tableQueryResult =
                    await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, resolver, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                ids.AddRange(tableQueryResult.Results);
            }
            while (continuationToken != null);

            return ids;
        }
    }
}