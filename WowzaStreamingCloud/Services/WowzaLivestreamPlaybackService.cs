using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities.StreamTargets;
using Swabbr.WowzaStreamingCloud.Tokens;
using Swabbr.WowzaStreamingCloud.Utility;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Manages playback for Wowza <see cref="Livestream"/>s.
    /// </summary>
    public sealed class WowzaLivestreamPlaybackService : ILivestreamPlaybackService
    {

        private readonly WowzaHttpClient _wowzaHttpClient;
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFollowRequestRepository _followRequestRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public WowzaLivestreamPlaybackService(IOptions<WowzaStreamingCloudConfiguration> wscOptions,
            ILivestreamRepository livestreamRepository,
            IVlogRepository vlogRepository,
            IUserRepository userRepository,
            IFollowRequestRepository followRequestRepository)
        {
            if (wscOptions == null || wscOptions.Value == null) { throw new ArgumentNullException(nameof(wscOptions)); }
            _wowzaHttpClient = new WowzaHttpClient(wscOptions.Value);
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _followRequestRepository = followRequestRepository ?? throw new ArgumentNullException(nameof(followRequestRepository));
        }

        /// <summary>
        /// Generates <see cref="ParametersFollowedProfileLive"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="ParametersFollowedProfileLive"/></returns>
        public async Task<ParametersFollowedProfileLive> GetDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            await _userRepository.GetAsync(watchingUserId).ConfigureAwait(false); // Throws if user doesn't exist
            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

            // TODO Do we need the vlog here?
            var vlog = await _vlogRepository.GetVlogFromLivestreamAsync(livestreamId).ConfigureAwait(false);

            // TODO Maybe have some more livestream state checks in here?
            if (!await _wowzaHttpClient.IsLivestreamStartedAsync(livestream.ExternalId))
            {
                throw new InvalidOperationException("Livestream isn't live"); // TODO Do we want this?
            }

            // TODO DRY
            //return new ParametersFollowedProfileLive
            //{
            //    EndpointUrl = await GetPlaybackUrlAsync(livestreamId).ConfigureAwait(false),
            //    LiveLivestreamId = livestreamId,
            //    LiveUserId = livestream.UserId,
            //    LiveVlogId = vlog.Id,
            //    Token = await GetTokenAsync(livestreamId, watchingUserId).ConfigureAwait(false)
            //};
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the playback url for a Wowza livestream through Fastly.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns></returns>
        public async Task<Uri> GetPlaybackUrlAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);
            var wscFastly = await GetFastlyAsync(livestream.ExternalId).ConfigureAwait(false);
            return wscFastly.StreamTargetFastly.PlaybackUrl;
        }

        /// <summary>
        /// Gets a token for a <see cref="Core.Entities.Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <param name="watchingUserId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns>Token</returns>
        public async Task<string> GetVlogTokenAsync(Guid livestreamId, Guid watchingUserId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            watchingUserId.ThrowIfNullOrEmpty();

            // TODO Implement vlog/livestream public/private properties 
            // var watchingUser = await _userRepository.GetAsync(watchingUserId).ConfigureAwait(false);

            var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

            // TODO Livestreams also return stream targets!!!
            var fastly = await GetFastlyAsync(livestream.ExternalId).ConfigureAwait(false);
            var fastlyLivestreamId = ExtractFastlyStreamId(fastly.StreamTargetFastly.PlaybackUrl);
            var fastlySecret = fastly.StreamTargetFastly.TokenAuthSharedSecret;

            return FastlyHmacGenerator.Generate(fastlyLivestreamId, fastlySecret);
        }

        /// <summary>
        /// Gets a <see cref="WscFastlyResponse"/> from the Wowza API.
        /// </summary>
        /// <param name="transcoderId">Wowza Transcoder id</param>
        /// <returns><see cref="WscFastlyResponse"/></returns>
        private async Task<WscFastlyResponse> GetFastlyAsync(string transcoderId)
        {
            var wscOutputs = await _wowzaHttpClient.GetOutputsAsync(transcoderId).ConfigureAwait(false);

            // Throws if the Wowza configuration is incorrect
            var outputs = await _wowzaHttpClient.GetOutputsAsync(transcoderId).ConfigureAwait(false);
            var matchingOutputs = outputs.Outputs.Where(x =>
                x.PassthroughVideo == WowzaConstants.OutputPassthroughVideo &&
                x.AspectRatioWidth == WowzaConstants.OutputAspectRatioWidth &&
                x.AspectRatioHeight == WowzaConstants.OutputAspectRatioHeight);
            if (!matchingOutputs.Any()) { throw new InvalidOperationException("Could not find correct Wowza Output (passthrough video, 1280x720)"); }
            if (matchingOutputs.Count() > 1) { throw new InvalidOperationException("Found more than one correct Wowza Output (passthrough video, 1280x720)"); }

            // Get the fastly stream target and return the playback url
            return await _wowzaHttpClient.GetFastlyAsync(matchingOutputs.First().OutputStreamTargets.First().StreamTargetId).ConfigureAwait(false);
        }

        /// <summary>
        /// Extracts the Fastly livestream id from the Fastly playback url. For
        /// some reason Wowza decided to use this Fastly livestream id for the 
        /// token generation instead of their own Wowza livestream id.
        /// </summary>
        /// <param name="playbackUrl">Fastly stream target playback url</param>
        /// <returns>Fastly livestream id</returns>
        private string ExtractFastlyStreamId(Uri playbackUrl)
        {
            playbackUrl.AbsoluteUri.ThrowIfNullOrEmpty();

            try
            {
                var pattern = @"^https:\/\/.+\.wowza\.com\/\d\/.+\/.+\/hls\/live\/playlist\.m3u8$";
                if (!Regex.IsMatch(playbackUrl.AbsoluteUri, pattern))
                {
                    throw new InvalidOperationException("Playback url does not match regex template");
                }

                // Split by wowza.com
                var splitByCom = playbackUrl.AbsoluteUri.Split(new string[] { ".wowza.com" }, StringSplitOptions.None);

                // Split by slash
                var splitBySlash = splitByCom[1].Split('/');
                return splitBySlash[2];
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("URL was malformed - unable to extract Fastly livestream id", e);
            }
        }

        Task<LivestreamDownstreamDetails> ILivestreamPlaybackService.GetLivestreamDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId)
        {
            throw new NotImplementedException();
        }

        public Task<VlogPlaybackDetails> GetVlogDownstreamParametersAsync(Guid vlogId, Guid watchingUserId)
        {
            throw new NotImplementedException();
        }
    }

}
