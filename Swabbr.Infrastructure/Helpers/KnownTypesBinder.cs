using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Infrastructure.Helpers
{
    // FUTURE Decouple from Newtonsoft.Json when System.Text.Json can handle polymorphic serialization.
    /// <summary>
    ///     Newtonsoft Json helper class which allows us to specify which
    ///     subclass types the Json parser can expect. Without specifying 
    ///     these subclasses the parser will always use the base class to
    ///     parse. This allows us to handle polymorphic serialization.
    /// </summary>
    public class KnownTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public Type BindToType(string assemblyName, string typeName) => KnownTypes.SingleOrDefault(t => t.Name == typeName);

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}
