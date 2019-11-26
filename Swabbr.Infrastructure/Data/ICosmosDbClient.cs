// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Interface for the Cosmos Db client.
    /// </summary>
    /// <typeparam name="T">Entity type of the client.</typeparam>
    public interface ICosmosDbClient<T>
    {
        // TODO: Documentation
        Task<T> InsertEntityAsync(T item);

        Task<T> RetrieveEntityAsync(string partitionKey, string rowKey);

        Task<T> UpdateEntityAsync(TableEntity item);

        Task<T> DeleteEntityAsync(string partitionKey, string rowKey);

        Task<IEnumerable<T>> QueryTableAsync(/*  TODO ??? */);
    }
}
