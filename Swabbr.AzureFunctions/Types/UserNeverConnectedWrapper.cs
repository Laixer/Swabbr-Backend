using System;

namespace Swabbr.AzureFunctions.Types
{

    /// <summary>
    /// JSON wrapper for <see cref="Functions.LogicAppUserNeverConnectedFunction"/>.
    /// </summary>
    public sealed class UserNeverConnectedWrapper
    {

        public Guid LivestreamId { get; set; }

        public Guid UserId { get; set; }

    }

}
