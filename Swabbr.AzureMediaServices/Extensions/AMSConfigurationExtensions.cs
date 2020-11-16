using Swabbr.AzureMediaServices.Configuration;
using Swabbr.Core.Extensions;
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

            config.AadClientId.ThrowIfNullOrEmpty();
            //if (config.AadEndpoint == null) { throw new ArgumentNullException(nameof(config.AadEndpoint)); } // TODO Check this
            config.AadSecret.ThrowIfNullOrEmpty();
            config.AadTenantId.ThrowIfNullOrEmpty();
            config.AccountName.ThrowIfNullOrEmpty();
            if (config.ArmAadAudience == null) { throw new ArgumentNullException(nameof(config.ArmAadAudience)); }
            if (config.ArmEndpoint == null) { throw new ArgumentNullException(nameof(config.ArmEndpoint)); }
            //config.Region.ThrowIfNullOrEmpty(); // TODO Fix and assign
            config.ResourceGroup.ThrowIfNullOrEmpty();
            config.SubscriptionId.ThrowIfNullOrEmpty();
            config.TokenAudience.ThrowIfNullOrEmpty();
            config.TokenIssuer.ThrowIfNullOrEmpty();
            config.TokenSecret.ThrowIfNullOrEmpty();
            if (config.TokenValidMinutes <= 0) { throw new ArgumentOutOfRangeException(nameof(config.TokenValidMinutes)); }
        }

    }

}
