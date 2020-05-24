using Swabbr.Core.Entities;
using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Wrapper for uploading a <see cref="Reaction"/>.
    /// </summary>
    public sealed class ReactionUploadWrapper
    {

        public Reaction Reaction { get; set; }

        public Uri UploadUrl { get; set; }

    }

}
