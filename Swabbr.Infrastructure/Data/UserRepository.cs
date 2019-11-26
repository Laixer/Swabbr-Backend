// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.using System

using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    public class UserRepository : CosmosDbRepository<UserEntity>, IUserRepository
    {
        private readonly ICosmosDbClientFactory _factory;

        public UserRepository(ICosmosDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Users";
        ////public override string GenerateId(UserDocument entity) => Guid.NewGuid().ToString();
        ////public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);

        /// <summary>
        /// Search for a user by checking if the search query (partially) matches their first name, last name or nickname.
        /// </summary>
        /// <param name="q">The search query that is supplied by the client.</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> SearchAsync(string q, uint offset, uint limit)
        {
            throw new NotImplementedException();
            try
            {
                var cosmosDbClient = _factory.GetClient<UserEntity>(TableName);

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
            catch
            {

            }

        }
    }
}
