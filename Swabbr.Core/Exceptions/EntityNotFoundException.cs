using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Exception indicating we could not find a given entity in our data store.
    /// </summary>
    public class EntityNotFoundException : Exception
    {

        public EntityNotFoundException() { }

        public EntityNotFoundException(string message) : base(message) { }


        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        public EntityNotFoundException(string message, string id)
            : base($"{message}, id = {id}") { }

        public EntityNotFoundException(Type entityType, string id)
            : base($"Entity type = {((entityType == null) ? "null" : entityType.GetType().ToString())}, id = {id}") { }

    }

}
