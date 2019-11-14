// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using Swabbr.Core.Models;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Represents an item container in a Cosmos DB database instance.
    /// </summary>
    /// <typeparam name="T">The entity type of the item container</typeparam>
    public interface IContainerContext<in T> where T : Entity
    {
        string ContainerName { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(string entityId);
    }
}
