using Microsoft.Azure.Cosmos.Table;

namespace Swabbr.Core.Interfaces
{
    public interface IVlogRepository : IRepository<TableEntity> { }
}
