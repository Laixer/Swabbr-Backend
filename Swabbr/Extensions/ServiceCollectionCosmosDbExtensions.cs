using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Extensions
{
    public static class ServiceCollectionCosmosDbExtensions
    {
        public static IServiceCollection AddCosmosDb(
            this IServiceCollection services,
            string connectionString,
            List<TableProperties> tables)
        {
            ////var client = new CosmosClient(connectionString, clientOptions: new CosmosClientOptions
            ////{
            ////    SerializerOptions = new CosmosSerializationOptions
            ////    {
            ////        IgnoreNullValues = true,
            ////        Indented = false,
            ////        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
            ////    }
            ////});

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Initialize and add the Cosmos Db client factory service
            var factory = new DbClientFactory(tables, storageAccount);
            factory.EnsureDbSetupAsync().Wait();

            services.AddSingleton<IDbClientFactory>(factory);

            return services;
        }
    }
}