using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// <see cref="Reaction"/> with extended download URI's.
    /// TODO Mark
    /// </summary>
    [Obsolete("Not used anymore, we use tokens now")]
    public sealed class ReactionWithDownload : Reaction
    {

        public Uri VideoAccessUri { get; set; }

        public Uri ThumbnailAccessUri { get; set; }

    }
}
