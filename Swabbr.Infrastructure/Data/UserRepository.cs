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

        public Task<User> GetByIdAsync(Guid userId)
        {
            // Partition key and row key are the same. TODO Change partition key if there is a
            // better alternative.
            var id = userId.ToString();
            return GetAsync(id, id);
        }

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name,
        /// last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        public Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            _ = _factory.GetClient<UserEntity>(TableName);

            // TODO Implement method

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
            return new User
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Country = entity.Country,
                Email = entity.Email,
                Gender = entity.Gender,
                IsPrivate = entity.IsPrivate,

                Latitude = entity.Latitude,
                Longitude = entity.Longitude,

                Nickname = entity.Nickname,
                Password = entity.Password,
                PhoneNumber = entity.PhoneNumber,
                ProfileImageUrl = entity.ProfileImageUrl,
                Timezone = entity.Timezone
            };
        }

        public override UserEntity Map(User entity)
        {
            return new UserEntity
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Country = entity.Country,
                Email = entity.Email,
                Gender = entity.Gender,
                IsPrivate = entity.IsPrivate,

                Latitude = entity.Latitude,
                Longitude = entity.Longitude,

                Nickname = entity.Nickname,
                Password = entity.Password,
                PhoneNumber = entity.PhoneNumber,
                ProfileImageUrl = entity.ProfileImageUrl,
                Timezone = entity.Timezone
            };
        }

        public override string ResolvePartitionKey(UserEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(UserEntity entity) => entity.UserId.ToString();
    }
}