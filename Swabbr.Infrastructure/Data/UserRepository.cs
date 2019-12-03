using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using Swabbr.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public class UserRepository : DbRepository<User, UserEntity>, IUserRepository
    {
        private readonly IDbClientFactory _factory;

        public UserRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Users";

        public async Task TempDeleteTables()
        {
            await _factory.DeleteAllTables();
        }

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name, last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        /// <returns></returns>
        public Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            _ = _factory.GetClient<UserEntity>(TableName);

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

        public override User Map(UserEntity entity)
        {
            // TODO: TEMPORARY
            return new User
            {
                FirstName = "TestIncorrect",
                LastName = "TestIncorrect"
            };
        }

        public override UserEntity Map(User entity)
        {
            // TODO: TEMPORARY
            return new UserEntity(entity.UserId)
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName
            };
        }
    }
}