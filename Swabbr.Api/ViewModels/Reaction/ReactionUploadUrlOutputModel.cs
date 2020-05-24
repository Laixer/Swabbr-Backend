using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Reaction
{

    /// <summary>
    /// Wrapper for displaying a <see cref="Core.Entities.Reaction"/> upload <see cref="Uri"/>.
    /// </summary>
    public sealed class ReactionUploadUrlOutputModel
    {

        public Uri UploadUri { get; set; }

    }

}
