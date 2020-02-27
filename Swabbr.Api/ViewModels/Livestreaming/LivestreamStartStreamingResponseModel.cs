using System;

namespace Swabbr.Api.ViewModels.Livestreaming
{

    /// <summary>
    /// The response object after we call <see cref="Controllers.LivestreamsController.StartStreamingAsync(Guid)"/>.
    /// </summary>
    public sealed class LivestreamStartStreamingResponseModel
    {

        /// <summary>
        /// References the <see cref="Core.Entities.Vlog"/> that belongs 
        /// to the current <see cref="Core.Entities.Livestream"/>.
        /// </summary>
        public Guid VlogId { get; set; }

    }

}
