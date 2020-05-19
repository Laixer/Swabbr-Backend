using Microsoft.Azure.Management.Media;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Swabbr.AzureMediaServices.Extensions;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Configuration
{

    /// <summary>
    /// Factory to build an Azure Media Services client.
    /// TODO Should this be a "singleton" like this? Does that support concurrency?
    /// </summary>
    internal sealed class AMSClientFactory
    {

        private static IAzureMediaServicesClient client;

        /// <summary>
        /// Creates a new instance of <see cref="AzureMediaServicesClient"/>.
        /// </summary>
        /// <param name="config"><see cref="AMSConfiguration"/></param>
        /// <returns><see cref="AzureMediaServicesClient"/></returns>
        internal static async Task<IAzureMediaServicesClient> GetClientAsync(AMSConfiguration config)
        {
            config.ThrowIfInvalid();

            if (client != null) { return client; }

            var clientCredential = new ClientCredential(config.AadClientId, config.AadSecret);
            var credentials = await ApplicationTokenProvider.LoginSilentAsync(config.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);

            client = new AzureMediaServicesClient(config.ArmEndpoint, credentials)
            {
                SubscriptionId = config.SubscriptionId,
            };
            return client;
        }

    }

}
