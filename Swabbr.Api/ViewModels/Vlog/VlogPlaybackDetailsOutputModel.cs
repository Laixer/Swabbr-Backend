using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Vlog
{
    
    /// <summary>
    /// Contains playback details for a <see cref="Core.Entities.Vlog"/>.
    /// </summary>
    public class VlogPlaybackDetailsOutputModel
    {

        public Guid VlogId { get; set; }

        public Uri EndpointUrl { get; set; }

        public string Token { get; set; }

    }

}
