using Swabbr.AzureMediaServices.Configuration;
using System;

namespace Swabbr.AzureMediaServices.Extensions
{

    /// <summary>
    /// Contains extension functionality for <see cref="AMSConfiguration"/>.
    /// </summary>
    public static class AMSConfigurationExtensions
    {

        public static void ThrowIfInvalid(this AMSConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            // TODO Do more
        }

    }

}
