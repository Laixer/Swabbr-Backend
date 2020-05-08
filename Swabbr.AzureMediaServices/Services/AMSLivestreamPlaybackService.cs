﻿using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
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
        private readonly IAMSClient _amsClient;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogService _vlogService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamPlaybackService(IOptions<AMSConfiguration> config,
            ILivestreamRepository livestreamRepository,
            IAMSClient amsClient,
            IVlogService vlogService)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
        }

        /// <summary>
        /// Gets our downstream parameters and token for <see cref="Core.Entities.Livestream"/>
        /// playback.
        /// </summary>
        /// <remarks>
        /// This does not take any public/private properties into account.
        /// TODO Optimize, duplicate calls here
        /// </remarks>
        /// <param name="livestreamId"></param>
        /// <param name="watchingUserId"></param>
        /// <returns></returns>
        public async Task<LivestreamDownstreamDetails> GetLivestreamDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            return new LivestreamDownstreamDetails
            {
                EndpointUrl = await GetPlaybackUrlAsync(livestreamId).ConfigureAwait(false),
                LiveLivestreamId = livestreamId,
                LiveUserId = (await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false)).UserId,
                LiveVlogId = (await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false)).Id,
                Token = await GetTokenByLivestreamAsync(livestreamId).ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Gets our downstream parameters and token for <see cref="Core.Entities.Vlog"/>
        /// playback.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="VlogPlaybackDetails"/></returns>
        public async Task<VlogPlaybackDetails> GetVlogDownstreamParametersAsync(Guid vlogId, Guid watchingUserId)
        {
            vlogId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            // Internal checks
            var vlog = await _vlogService.GetAsync(vlogId).ConfigureAwait(false);
            if (!vlog.LivestreamId.IsNullOrEmpty()) { throw new LivestreamStateException("Vlog is still linked to livestream"); }
            // TODO Check if the user is allowed to watch the vlog

            var hostName = await _amsClient.GetStreamingEndpointHostNameAsync().ConfigureAwait(false);
            var endpointUrl = (await _amsClient.GetVlogStreamingLocatorPathsAsync(vlogId).ConfigureAwait(false)).FirstOrDefault();

            return new VlogPlaybackDetails
            {
                VlogId = vlogId,
                EndpointUrl = endpointUrl == null ? null : new Uri($"{hostName}{endpointUrl}"),
                Token = await GetVlogTokenAsync(vlogId).ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Retrieves the playback url for a <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Uri"/></returns>
        private async Task<Uri> GetPlaybackUrlAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
            if (livestream.LivestreamStatus != LivestreamStatus.Live) { throw new LivestreamStateException($"Livestream not in {LivestreamStatus.Live.GetEnumMemberAttribute()} state"); }

            var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

            var paths = await _amsClient.GetVlogStreamingLocatorPathsAsync(vlog.Id).ConfigureAwait(false);
            if (!paths.Any()) { return null; } // TODO How to fix this?

            var hostName = await _amsClient.GetStreamingEndpointHostNameAsync().ConfigureAwait(false);

            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = hostName,
                Path = paths.First()
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Generates a JWT token for livestream playback.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns>JWT token <see cref="string"/></returns>
        private async Task<string> GetTokenByLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            // Get corresponding vlog
            var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

            var tokenSigningKey = new SymmetricSecurityKey(new UTF8Encoding().GetBytes(_config.TokenSecret));
            var credentials = new SigningCredentials(tokenSigningKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest);
            var keyIdentifier = await _amsClient.GetVlogStreamingLocatorKeyIdentifierAsync(vlog.Id).ConfigureAwait(false);
            var claims = new Claim[] { new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier) };

            var token = new JwtSecurityToken(
                issuer: _config.TokenIssuer,
                audience: _config.TokenAudience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddMinutes(-5),
                expires: DateTime.UtcNow.AddMinutes(_config.TokenValidMinutes),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// TODO Duplicate function
        /// </summary>
        /// <param name="livestreamId"></param>
        /// <returns>JWT Token</returns>
        private async Task<string> GetVlogTokenAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            var tokenSigningKey = new SymmetricSecurityKey(new UTF8Encoding().GetBytes(_config.TokenSecret));
            var credentials = new SigningCredentials(tokenSigningKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest);
            var keyIdentifier = await _amsClient.GetVlogStreamingLocatorKeyIdentifierAsync(vlogId).ConfigureAwait(false);
            var claims = new Claim[] { new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier) };

            var token = new JwtSecurityToken(
                issuer: _config.TokenIssuer,
                audience: _config.TokenAudience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddMinutes(-5),
                expires: DateTime.UtcNow.AddMinutes(_config.TokenValidMinutes),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
