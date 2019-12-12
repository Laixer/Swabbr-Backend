using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
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
            // Partition key and row key are the same. TODO Change partition key if there is a
            // better alternative.
            var id = userId.ToString();
            return GetAsync(id, id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName);

            var queryResults = await table.Query(new TableQuery().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email))
            );

            if (queryResults.Any())
            {
                return Map(queryResults.First());
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name,
        /// last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        public async Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName);

            var queryResults = await table.Query(new TableQuery().Where(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("FirstName", QueryComparisons.Equal, q),
                            TableOperators.Or,
                            TableQuery.GenerateFilterCondition("Username", QueryComparisons.Equal, q)
                            )
                        ));

            // TODO Implement method

            throw new NotImplementedException();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName);

            var queryResults = await table.Query(new TableQuery().Where(
                TableQuery.GenerateFilterCondition("Username", QueryComparisons.Equal, username))
            );

            if (queryResults.Any())
            {
                return Map(queryResults.First());
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public override User Map(UserTableEntity entity)
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
                PasswordHash = entity.PasswordHash,
                PhoneNumber = entity.PhoneNumber,
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
                BirthDate = entity.BirthDate,
                Country = entity.Country,
                Email = entity.Email,
                Gender = entity.Gender,
                IsPrivate = entity.IsPrivate,

                Latitude = entity.Latitude,
                Longitude = entity.Longitude,

                Nickname = entity.Nickname,
                PasswordHash = entity.PasswordHash,
                PhoneNumber = entity.PhoneNumber,
                ProfileImageUrl = entity.ProfileImageUrl,
                Timezone = entity.Timezone
            };
        }

        public override string ResolvePartitionKey(UserTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(UserTableEntity entity) => entity.UserId.ToString();
    }
}