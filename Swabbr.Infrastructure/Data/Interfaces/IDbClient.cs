// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    /// <summary>
    /// Interface for the Cosmos Db client.
    /// </summary>
    public interface IDbClient<T>
    {
        // TODO: Documentation
        Task<T> InsertEntityAsync(TableEntity item);

        Task<T> RetrieveEntityAsync(string partitionKey, string rowKey);

        Task<T> UpdateEntityAsync(TableEntity item);

        Task<T> DeleteEntityAsync(string partitionKey, string rowKey);

        Task<IEnumerable<T>> QueryTableAsync(/*  TODO  what parameters??? */);
    }
}
