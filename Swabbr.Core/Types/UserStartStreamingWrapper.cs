using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// JSON wrapper for a user-start-streaming request.
    /// </summary>
    public sealed class UserStartStreamingWrapper
    {

        public Guid UserId { get; set; }

        public Guid LivestreamId { get; set; }

        public uint UserConnectTimeoutSeconds { get; set; }

    }

}
