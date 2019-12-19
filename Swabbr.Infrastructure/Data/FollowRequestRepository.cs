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

namespace Swabbr.Infrastructure.Data
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
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).CloudTableReference;

            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                TableQuery.GenerateFilterCondition("FollowRequestId", QueryComparisons.Equal, followRequestId.ToString()));

            var queryResults = table.ExecuteQuery(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Task.FromResult(Map(queryResults.First()));
        }

        //TODO Make async
        public Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId)
        {
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).CloudTableReference;

            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                TableQuery.GenerateFilterCondition("ReceiverId", QueryComparisons.Equal, userId.ToString()));

            var queryResults = table.ExecuteQuery(tq);
            var mappedResults = queryResults.Select(x => Map(x));

            return Task.FromResult(mappedResults);
        }

        public Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId)
        {
            var table = _factory.GetClient<FollowRequestTableEntity>(TableName).CloudTableReference;

            var tq = new TableQuery<FollowRequestTableEntity>().Where(
                TableQuery.GenerateFilterCondition("RequesterId", QueryComparisons.Equal, userId.ToString()));

            var queryResults = table.ExecuteQuery(tq);
            var mappedResults = queryResults.Select(x => Map(x));

            return Task.FromResult(mappedResults);
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
    }
}