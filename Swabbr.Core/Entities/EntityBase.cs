using System;

namespace Swabbr.Core.Entities
{
    
    //public abstract class EntityBase { }

    /// <summary>
    /// Used as a base class for all our entities.
    /// </summary>
    public abstract class EntityBase<TPrimary>// : EntityBase
    {

        /// <summary>
        /// Internal entity unique identifier.
        /// </summary>
        public TPrimary Id { get; set; }

    }

}