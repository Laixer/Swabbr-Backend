using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Contains parameters we need to be able to start streaming to a livestream.
    /// </summary>
    public sealed class ParametersRecordVlog : ParametersJsonBase
    {

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
