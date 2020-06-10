using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Interfaces.Services;
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
    public sealed class AMSPlaybackService : IPlaybackService
    {
        private readonly AMSConfiguration _config;
        private readonly IAMSClient _amsClient;
        private readonly IReactionService _reactionService;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogService _vlogService;
        private readonly IAMSTokenService _amsTokenService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSPlaybackService(IOptions<AMSConfiguration> config,
            ILivestreamRepository livestreamRepository,
            IReactionService reactionService,
            IAMSClient amsClient,
            IVlogService vlogService,
            IAMSTokenService amsTokenService)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            _amsTokenService = amsTokenService ?? throw new ArgumentNullException(nameof(amsTokenService));
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
                Token = await GetLivestreamTokenAsync(livestreamId).ConfigureAwait(false)
            };
        }

        /// <summary>
        /// Gets our downstream parameters and token for <see cref="Core.Entities.Reaction"/>
        /// playback.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="VlogPlaybackDetails"/></returns>
        public async Task<ReactionPlaybackDetails> GetReactionDownstreamParametersAsync(Guid reactionId, Guid watchingUserId)
        {
            reactionId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            // Internal checks
            var reaction = await _reactionService.GetReactionAsync(reactionId).ConfigureAwait(false);
            if (reaction.ReactionState != ReactionState.Finished) { throw new ReactionStateException($"Reaction not in {ReactionState.Finished.GetEnumMemberAttribute()} state"); }

            // TODO Check if the user is allowed to watch the vlog

            var hostName = await _amsClient.GetStreamingEndpointHostNameAsync().ConfigureAwait(false);
            var endpointUrl = (await _amsClient.GetReactionStreamingLocatorPathsAsync(reactionId).ConfigureAwait(false)).FirstOrDefault();

            return new ReactionPlaybackDetails
            {
                ReactionId = reactionId,
                EndpointUrl = endpointUrl == null ? null : new Uri(hostName, endpointUrl),
                Token = await GetReactionTokenAsync(reactionId).ConfigureAwait(false)
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
            var token = await GetVlogTokenAsync(vlogId).ConfigureAwait(false);

            // Update view if we can watch the vlog
            if (endpointUrl != null) { await _vlogService.AddView(vlogId).ConfigureAwait(false); }

            return new VlogPlaybackDetails
            {
                VlogId = vlogId,
                EndpointUrl = endpointUrl == null ? null : new Uri(hostName, endpointUrl),
                Token = token
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
            if (livestream.LivestreamState != LivestreamState.Live) { throw new LivestreamStateException($"Livestream not in {LivestreamState.Live.GetEnumMemberAttribute()} state"); }

            var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

            var paths = await _amsClient.GetVlogStreamingLocatorPathsAsync(vlog.Id).ConfigureAwait(false);
            if (!paths.Any()) { return null; }

            return new Uri(await _amsClient.GetStreamingEndpointHostNameAsync().ConfigureAwait(false), paths.First());
        }

        private async Task<string> GetLivestreamTokenAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            var vlog = await _vlogService.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);
            return await GetVlogTokenAsync(vlog.Id).ConfigureAwait(false);
        }

        private async Task<string> GetVlogTokenAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            return _amsTokenService.GenerateToken(await _amsClient.GetVlogStreamingLocatorKeyIdentifierAsync(vlogId).ConfigureAwait(false));
        }

        private async Task<string> GetReactionTokenAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return _amsTokenService.GenerateToken(await _amsClient.GetReactionStreamingLocatorKeyIdentifierAsync(reactionId).ConfigureAwait(false));
        }
    }
}
