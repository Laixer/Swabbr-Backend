using Swabbr.Infrastructure.Data;
using System.Collections.Generic;

namespace Swabbr.Api.Options
{
    public class CosmosDbOptions
    {
        public List<TableProperties> Tables { get; set; }

        public void Deconstruct(out List<TableProperties> tables) {
            tables = Tables;
        }
    }
}