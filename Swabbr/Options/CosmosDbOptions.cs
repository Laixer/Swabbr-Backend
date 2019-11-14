using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Options
{
    public class CosmosDbOptions
    {
        public string DatabaseName { get; set; }
        public List<ContainerProperties> Containers { get; set; }

        public void Deconstruct(out string databaseName, out List<ContainerProperties> containers)
        {
            databaseName = DatabaseName;
            containers = Containers;
        }
    }
}