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
    public class UserRepository : DbRepository<SwabbrUser, UserTableEntity>, IUserRepository
    {
        private readonly IDbClientFactory _factory;

        public UserRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Users";

        public Task<SwabbrUser> GetAsync(Guid userId)
        {
            // Partition key and row key are the same.
            //TODO: Change partition key if there is a better alternative.
            var id = userId.ToString();
            return RetrieveAsync(id, id);
        }

        public async Task<SwabbrUser> GetByEmailAsync(string email)
        {
            var tableQuery = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            var queryResults = await QueryAsync(tableQuery);

            if (queryResults.Any())
            {
                return Map(queryResults.First());
            }

            throw new EntityNotFoundException();
        }

        /// <summary>
        /// Search for a user by checking if the given search query matches the beginning of their
        /// first name, last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        public async Task<IEnumerable<SwabbrUser>> SearchAsync(string q, uint offset, uint limit)
        {
            //TODO: Use cognitive search

            //! IMPORTANT

            //TODO: Currently returning all rows... not ideal
            var tableQuery = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool("PhoneNumberConfirmed", QueryComparisons.Equal, false));

            var queryResults = await QueryAsync(tableQuery);

            return queryResults.Select(x => Map(x));
        }

        public override SwabbrUser Map(UserTableEntity entity)
        {
            throw new NotImplementedException();
        }

        public override UserTableEntity Map(SwabbrUser entity)
        {
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(UserTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(UserTableEntity entity) => entity.UserId.ToString();

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var tableQuery = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("PartitionKey", QueryComparisons.Equal, userId));

            return await GetEntityCountAsync(tableQuery) > 0;
        }
    }
}