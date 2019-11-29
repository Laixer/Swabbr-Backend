using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using Swabbr.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public class UserRepository : DbRepository<UserEntity>, IUserRepository
    {
        private readonly IDbClientFactory _factory;

        public UserRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Users";

        // TODO: Is this needed here? Will it be different per entity?
        public override string GenerateId(UserEntity entity)
        {
            return new Guid().ToString();
        }

        //TODO: Remove this... Not required
        public override string ResolvePartitionKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public override string ResolveRowKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public async Task TempDeleteTables()
        {
            // TODO Delete tables
            await _factory.DeleteAllTables();
        }

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name, last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> SearchAsync(string q, uint offset, uint limit)
        {
            var client = _factory.GetClient<UserEntity>(TableName);

            throw new NotImplementedException();

            ////var query = new QueryDefinition(@"
            ////    SELECT * FROM c 
            ////    WHERE CONTAINS(c.firstName, @firstName) 
            ////    OR CONTAINS(c.lastName, @lastName) 
            ////    OR CONTAINS(c.nickname, @nickname) 
            ////    OFFSET @offset LIMIT @limit")
            ////    .WithParameter("@firstName", q)
            ////    .WithParameter("@lastName", q)
            ////    .WithParameter("@nickname", q)
            ////    .WithParameter("@offset", offset)
            ////    .WithParameter("@limit", limit);
            ////
            ////var results = await cosmosDbClient.QueryAsync(query);
            ////return results;
        }
    }
}
