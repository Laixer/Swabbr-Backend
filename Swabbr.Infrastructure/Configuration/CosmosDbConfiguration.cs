using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Configuration
{
    public class CosmosDbConfiguration
    {
        public List<TableProperties> Tables { get; set; }

        public ConnectionStringsConfiguration ConnectionStrings { get; set; }

        public void Deconstruct(out List<TableProperties> tables, out ConnectionStringsConfiguration connectionStrings)
        {
            tables = Tables;
            connectionStrings = ConnectionStrings;
        }
    }
}