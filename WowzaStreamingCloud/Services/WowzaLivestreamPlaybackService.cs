using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities.StreamTargets;
using Swabbr.WowzaStreamingCloud.Tokens;
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
        /// Gets the playback url for a Wowza livestream through Fastly.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns></returns>
        public async Task<string> GetPlaybackUrlAsync(Guid livestreamId)
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
        public async Task<string> GetTokenAsync(Guid livestreamId, Guid watchingUserId)
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

            // Throw if the Wowza configuration is incorrect
            if (!wscOutputs.Outputs.Any()) { throw new ExternalErrorException("Wowza Transcoder has no Outputs"); }
            if (wscOutputs.Outputs.Count() > 1) { throw new ExternalErrorException("Wowza Transcoder has more than one Output"); }
            var output = wscOutputs.Outputs.First();
            if (!output.OutputStreamTargets.Any()) { throw new ExternalErrorException("Wowza Output has no Output Stream Targets"); }
            if (output.OutputStreamTargets.Count() > 1) { throw new ExternalErrorException("Wowza Output has more than one Output Stream Target"); }

            // Get the fastly stream target and return the playback url
            return await _wowzaHttpClient.GetFastlyAsync(output.OutputStreamTargets.First().StreamTargetId).ConfigureAwait(false);
        }

        /// <summary>
        /// Extracts the Fastly livestream id from the Fastly playback url. For
        /// some reason Wowza decided to use this Fastly livestream id for the 
        /// token generation instead of their own Wowza livestream id.
        /// </summary>
        /// <param name="playbackUrl">Fastly stream target playback url</param>
        /// <returns>Fastly livestream id</returns>
        private string ExtractFastlyStreamId(string playbackUrl)
        {
            playbackUrl.ThrowIfNullOrEmpty();

            try
            {
                var pattern = @"^https:\/\/.+\.wowza\.com\/\d\/.+\/.+\/hls\/live\/playlist\.m3u8$";
                if (!Regex.IsMatch(playbackUrl, pattern))
                {
                    throw new InvalidOperationException("Playback url does not match regex template");
                }

                // Split by wowza.com
                var splitByCom = playbackUrl.Split(new string[] { ".wowza.com" }, StringSplitOptions.None);

                // Split by slash
                var splitBySlash = splitByCom[1].Split('/');
                return splitBySlash[2];
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("URL was malformed - unable to extract Fastly livestream id", e);
            }
        }

    }

}
