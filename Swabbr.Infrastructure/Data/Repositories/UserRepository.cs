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
    public class UserRepository : DbRepository<User, UserTableEntity>, IUserRepository
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
            // Partition key and row key are the same.
            //TODO Change partition key if there is a better alternative.
            var id = userId.ToString();
            return RetrieveAsync(id, id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName).TableReference;

            var tq = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            var queryResults = await table.ExecuteQueryAsync(tq);

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
        public async Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            //TODO Use cognitive search

            //! IMPORTANT

            //TODO Currently returning all rows... not ideal
            var table = _factory.GetClient<UserTableEntity>(TableName).TableReference;

            var tq = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool("PhoneNumberConfirmed", QueryComparisons.Equal, false));

            var queryResults = await table.ExecuteQueryAsync(tq);

            return queryResults.Select(x => Map(x));
        }

        public override User Map(UserTableEntity entity)
        {
            return new User
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,

                Email = entity.Email,

                BirthDate = entity.BirthDate,
                Country = entity.Country,
                Gender = (Gender)entity.Gender,
                IsPrivate = entity.IsPrivate,

                Latitude = entity.Latitude,
                Longitude = entity.Longitude,

                Nickname = entity.Nickname,
                ProfileImageUrl = entity.ProfileImageUrl,
                Timezone = entity.Timezone
            };
        }

        public override UserTableEntity Map(User entity)
        {
            return new UserTableEntity
            {
                UserId = entity.UserId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,

                Email = entity.Email,

                BirthDate = entity.BirthDate,
                Country = entity.Country,
                Gender = (int)entity.Gender,
                IsPrivate = entity.IsPrivate,

                Latitude = entity.Latitude,
                Longitude = entity.Longitude,

                Nickname = entity.Nickname,
                ProfileImageUrl = entity.ProfileImageUrl,
                Timezone = entity.Timezone
            };
        }

        public override string ResolvePartitionKey(UserTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(UserTableEntity entity) => entity.UserId.ToString();

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var tq = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("PartitionKey", QueryComparisons.Equal, userId));

            return await GetEntityCountAsync(tq) > 0;
        }
    }
}