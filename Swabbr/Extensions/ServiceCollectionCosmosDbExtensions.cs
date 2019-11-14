using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Extensions
{
    public static class ServiceCollectionCosmosDbExtensions
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, string connectionString,
            string databaseName, List<Infrastructure.Data.ContainerProperties> collections)
        {
            var client = new CosmosClient(connectionString, clientOptions: new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    IgnoreNullValues = true,
                    Indented = false,
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                }
            });

            var cosmosDbClientFactory = new CosmosDbClientFactory(databaseName, collections, client);
            cosmosDbClientFactory.EnsureDbSetupAsync().Wait();

            services.AddSingleton<ICosmosDbClientFactory>(cosmosDbClientFactory);

            return services;
        }
    }
}
