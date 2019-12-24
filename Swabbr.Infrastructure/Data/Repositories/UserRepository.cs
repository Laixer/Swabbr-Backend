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
        private struct UserIdWithName
        {
            public Guid UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
        }

        private List<UserIdWithName> userIdsWithNames = new List<UserIdWithName>();

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
        /// Search for a user by checking if the given search query matches the beginning of their first name,
        /// last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        public async Task<IEnumerable<User>> SearchAsync(string q, uint offset, uint limit)
        {
            if (q.Length <= 0)
                return null;

            var qNormalized = q.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            
            var table = _factory.GetClient<UserTableEntity>(TableName).CloudTableReference;

            // The filter that is currently used is similar to a T-SQL 'LIKE' query with a wildcard at the end of the query.
            //! Important: Only the right side of the search query can be tested against the table. This has the same functionality as a 'StartsWith' function.
            string constructColumnFilter(string column) {
                var lastChar = q[q.Length - 1];

                // Similar to 'SELECT WHERE <column> LIKE '<query>%'
                return TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(column, QueryComparisons.GreaterThanOrEqual, q),
                    TableOperators.And,
                    // Column must be less than or equal to (lte) the query + last character incremented by 1. 
                    TableQuery.GenerateFilterCondition(column, QueryComparisons.LessThanOrEqual, $"{q}{char.ToString((char)(lastChar + 1))}")
                );
            }

            var tq = new TableQuery<UserTableEntity>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            constructColumnFilter("FirstName"),
                            TableOperators.Or,
                            constructColumnFilter("LastName")
                        ),
                        TableOperators.Or,
                        constructColumnFilter("Nickname")
                    )
            );

            var queryResults = table.ExecuteQuery(tq);

            return queryResults.Select(x => Map(x));
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
    }
}