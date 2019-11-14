// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;

namespace Swabbr.Infrastructure.Data
{
    public class UserRepository : CosmosDbRepository<UserItem> , IUserRepository
    {
        private readonly ICosmosDbClientFactory _factory;

        public UserRepository(ICosmosDbClientFactory factory) : base(factory) {
            _factory = factory;
        }

        public override string ContainerName { get; } = "Users";
        public override string GenerateId(UserItem entity) => Guid.NewGuid().ToString();
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name, last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserItem>> SearchAsync(string q, uint offset = 0, uint limit = 1)
        {
            try
            {
                var cosmosDbClient = _factory.GetClient<UserItem>(ContainerName);

                var query = new QueryDefinition(@"
                    SELECT * FROM c 
                    WHERE CONTAINS(c.firstName, @firstName) 
                    OR CONTAINS(c.lastName, @lastName) 
                    OR CONTAINS(c.nickname, @nickname) 
                    OFFSET @offset LIMIT @limit")
                    .WithParameter("@firstName", q)
                    .WithParameter("@lastName", q)
                    .WithParameter("@nickname", q)
                    .WithParameter("@offset", offset)
                    .WithParameter("@limit", limit);

                var results = await cosmosDbClient.QueryAsync(query);
                return results;
            }
            catch (CosmosException e)
            {
                // TODO ....
                throw;
            }
        }
    }
}
