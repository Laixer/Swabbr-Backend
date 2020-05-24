using System;
using System.IO;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Wrapper for a <see cref="Stream"/> and a <see cref="Guid"/> of an
    /// <see cref="Entity"/>.
    /// </summary>
    public sealed class StreamWithEntityIdWrapper : IDisposable
    {

        public StreamWithEntityIdWrapper(Stream stream, Guid entityId)
        {
            Stream = stream;
            EntityId = entityId;
        }

        public Stream Stream { get; set; }

        public Guid EntityId { get; set; }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }

}
