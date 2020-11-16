using Swabbr.Core.Extensions;
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
            pars.LivestreamId.ThrowIfNullOrEmpty();
            if (pars.RequestMoment == null) { throw new ArgumentNullException(nameof(pars.RequestMoment)); }
            if (pars.RequestTimeout == null) { throw new ArgumentNullException(nameof(pars.RequestTimeout)); }
            pars.VlogId.ThrowIfNullOrEmpty();
        }

    }
}
