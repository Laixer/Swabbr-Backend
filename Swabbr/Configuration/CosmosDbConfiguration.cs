using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Options
{
    public class CosmosDbConfiguration
    {
        public List<TableProperties> Tables { get; set; }

        public ConnectionStringConfiguration ConnectionStrings { get; set; }

        public void Deconstruct(out List<TableProperties> tables, out ConnectionStringConfiguration connectionStrings)
        {
            tables = Tables;
            connectionStrings = ConnectionStrings;
        }
    }
}