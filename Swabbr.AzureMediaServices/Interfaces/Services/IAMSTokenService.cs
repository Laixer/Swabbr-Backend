using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Interfaces.Services
{
    /// <summary>
    /// Contract for generating tokens for playback in AMS.
    /// </summary>
    public interface IAMSTokenService
    {
        /// <summary>
        /// Generates a new token for a streaming locator.
        /// </summary>
        /// <param name="keyIdentifier">Streaming locator key identifier</param>
        /// <returns>Token</returns>
        public string GenerateToken(string keyIdentifier);
    }
}
