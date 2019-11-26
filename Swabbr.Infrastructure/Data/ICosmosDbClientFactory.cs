// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Swabbr.Infrastructure.Data
{
    public interface ICosmosDbClientFactory
    {
        ICosmosDbClient<T> GetClient<T>(string tableName);
    }
}
