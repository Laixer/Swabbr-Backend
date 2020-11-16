namespace Swabbr.Core.Entities
{
    // TODO Default TPrimary?
    /// <summary>
    ///     Used as a base class for all our entities.
    /// </summary>
    public abstract class EntityBase<TPrimary>
    {
        /// <summary>
        ///     Internal entity unique identifier.
        /// </summary>
        public TPrimary Id { get; set; }
    }
}
