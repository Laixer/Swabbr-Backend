using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Service for managing <see cref="Livestream"/> playback.
    /// </summary>
    public sealed class AMSLivestreamPlaybackService : ILivestreamPlaybackService
    {

        private readonly AMSConfiguration _config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamPlaybackService(IOptions<AMSConfiguration> config)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
        }

        public Task<ParametersFollowedProfileLive> GetDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GetPlaybackUrlAsync(Guid livestreamId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a JWT token for livestream playback.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns>JWT token <see cref="string"/></returns>
        public async Task<string> GetTokenAsync(Guid livestreamId, Guid watchingUserId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
            var tokenSigningKey = new SymmetricSecurityKey(new UTF8Encoding().GetBytes(_config.TokenSecret));
            var credentials = new SigningCredentials(tokenSigningKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest);
            var keyIdentifier = await GetLocatorKeyIdentifierAsync(livestreamId).ConfigureAwait(false);
            var claims = new Claim[] { new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier) };

            var token = new JwtSecurityToken(
                issuer: _config.TokenIssuer,
                audience: _config.TokenAudience,
                claims: claims,
                notBefore: DateTime.Now.AddMinutes(-5),
                expires: DateTime.Now.AddMinutes(_config.TokenValidMinutes),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Retrieves the key from a <see cref="StreamingLocator"/> that belongs
        /// to a <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns>Key as <see cref="string"/></returns>
        private async Task<string> GetLocatorKeyIdentifierAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var streamingLocatorName = AMSNameGenerator.StreamingLocatorName(livestreamId);
            var streamingLocatorResponse = await amsClient.StreamingLocators.GetWithHttpMessagesAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName).ConfigureAwait(false);

            var streamingLocatorJson = await streamingLocatorResponse.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var streamingLocator = JsonConvert.DeserializeObject<StreamingLocator>(streamingLocatorJson);
            return streamingLocator.ContentKeys.First().Id.ToString();
        }

    }

}
