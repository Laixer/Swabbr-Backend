// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    public interface IDbClientFactory
    {
        IDbClient<T> GetClient<T>(string tableName);

        Task DeleteAllTables();
    }
}
