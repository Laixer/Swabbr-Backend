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
            return RetrieveAsync(id, id);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName).CloudTableReference;

            var tq = new TableQuery<UserTableEntity>().Where(
                TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email));

            var queryResults = table.ExecuteQuery(tq);

            if (queryResults.Any())
            {
                return Task.FromResult(Map(queryResults.First()));
            }

            throw new EntityNotFoundException();
        }

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name,
        /// last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        public Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            var table = _factory.GetClient<UserTableEntity>(TableName).CloudTableReference;

            //TODO Comments for querycomparisons!!!!!
            //! Important: Only right side can be tested this way
            var tq = new TableQuery<UserTableEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("FirstName", QueryComparisons.GreaterThanOrEqual, q),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("FirstName", QueryComparisons.LessThanOrEqual, q + "0")
                    ),
                    TableOperators.Or,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("LastName", QueryComparisons.GreaterThanOrEqual, q),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("LastName", QueryComparisons.LessThanOrEqual, q + "0")
                    )
                )
            );

            var queryResults = table.ExecuteQuery(tq);

            return Task.FromResult(queryResults.Select(x => Map(x)));
        }

        //TODO Check mapping
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