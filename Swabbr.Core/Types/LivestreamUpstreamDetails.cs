using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains upstream details for a <see cref="Core.Entities.Livestream"/>.
    /// </summary>
    public sealed class LivestreamUpstreamDetails
    {

        /// <summary>
        /// References the <see cref="Core.Entities.Vlog"/> that belongs 
        /// to the current <see cref="Core.Entities.Livestream"/>.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// References the <see cref="Core.Entities.Livestream"/>.
        /// </summary>
        public Guid LivestreamId { get; set; }

        /// <summary>
        /// Upstream endpoint url.
        /// </summary>
        public Uri HostServer { get; set; }

        /// <summary>
        /// Upstream endpoint port.
        /// </summary>
        public int HostPort { get; set; }

        /// <summary>
        /// Upstream endpoint application name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Upstream stream key.
        /// </summary>
        public string StreamKey { get; set; }

        /// <summary>
        /// Upstream authentication username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Upstream authentication password.
        /// </summary>
        public string Password { get; set; }

    }

}
