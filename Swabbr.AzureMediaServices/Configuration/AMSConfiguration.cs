using System;

namespace Swabbr.AzureMediaServices.Configuration
{

    /// <summary>
    /// Contains our configuration for Azure Media Services.
    /// </summary>
    public sealed class AMSConfiguration
    {

        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public string AccountName { get; set; }

        public string AadTenantId { get; set; }

        public string AadClientId { get; set; }

        public string AadSecret { get; set; }

        public Uri ArmAadAudience { get; set; }

        public Uri AadEndpoint { get; set; }

        public Uri ArmEndpoint { get; set; }

        public string Location { get; set; }

        public string TokenSecret { get; set; }

        public int TokenValidMinutes { get; set; }

        public string TokenIssuer { get; set; }

        public string TokenAudience { get; set; }

    }
}
