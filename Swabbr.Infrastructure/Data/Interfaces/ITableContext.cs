namespace Swabbr.Infrastructure.Data.Interfaces
{
    /// <summary>
    /// Represents an Azure CosmosDb Table
    /// </summary>
    public interface ITableContext
    {
        /// <summary>
        /// The name of the entity table.
        /// </summary>
        string TableName { get; }
    }
}