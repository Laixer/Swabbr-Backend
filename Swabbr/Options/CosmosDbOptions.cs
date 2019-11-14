using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Options
{
    public class CosmosDbOptions
    {
        public string DatabaseName { get; set; }
        public List<ContainerProperties> Collections { get; set; }

        public void Deconstruct(out string databaseName, out List<ContainerProperties> collections)
        {
            databaseName = DatabaseName;
            collections = Collections;
        }
    }
}