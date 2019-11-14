namespace Swabbr.Infrastructure.Data
{
    /// <summary>
    /// Properties for a Cosmos Db container.
    /// </summary>
    public class ContainerProperties
    {
        public string Name { get; set; }
        public string PartitionKey { get; set; }
    }
}