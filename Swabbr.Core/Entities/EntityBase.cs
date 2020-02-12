using System;

namespace Swabbr.Core.Entities
{
    
    /// <summary>
    /// Used as a base class for all our entities.
    /// </summary>
    public abstract class EntityBase
    {

        /// <summary>
        /// Internal entity unique identifier.
        /// </summary>
        public Guid Id { get; set; }

    }

}