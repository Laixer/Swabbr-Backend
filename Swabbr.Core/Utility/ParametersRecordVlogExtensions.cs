using Laixer.Utility.Extensions;
using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extension functionality for the <see cref="ParametersRecordVlog"/>
    /// class.
    /// </summary>
    public static class ParametersRecordVlogExtensions
    {

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if any fields are missing.
        /// </summary>
        /// <param name="pars"><see cref="ParametersRecordVlog"/></param>
        public static void Validate(this ParametersRecordVlog pars)
        {
            if (pars == null) { throw new ArgumentNullException(nameof(pars)); }
            pars.ApplicationName.ThrowIfNullOrEmpty();
            if (pars.HostPort == 0) { throw new ArgumentNullException(nameof(pars.HostPort)); }
            pars.Password.ThrowIfNullOrEmpty();
            if (pars.HostServer == null) { throw new ArgumentNullException(nameof(pars.HostServer)); }
            pars.StreamKey.ThrowIfNullOrEmpty();
            pars.Username.ThrowIfNullOrEmpty();
        }

    }
}
