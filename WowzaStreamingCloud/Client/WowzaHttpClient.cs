using Laixer.Utility.Extensions;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities;
using Swabbr.WowzaStreamingCloud.Entities.StreamTargets;
using Swabbr.WowzaStreamingCloud.Entities.Outputs;
using Swabbr.WowzaStreamingCloud.Enums;
using Swabbr.WowzaStreamingCloud.Parsing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Swabbr.WowzaStreamingCloud.Entities.Livestreams;

namespace Swabbr.WowzaStreamingCloud.Client
{

    /// <summary>
    /// Contains functionality to communicate and invoke the Wowza API. This is 
    /// a wrapper for the API, which also polls our livestream status when this
    /// is required.
    /// </summary>
    internal sealed partial class WowzaHttpClient : IDisposable
    {

        private readonly HttpClient httpClient = new HttpClient();
        private readonly WowzaStreamingCloudConfiguration WscOptions;
        private readonly Uri ApiBase;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public WowzaHttpClient(WowzaStreamingCloudConfiguration options, Uri apiBase)
        {
            WscOptions = options ?? throw new ArgumentNullException(nameof(options));
            ApiBase = apiBase ?? throw new ArgumentNullException(nameof(apiBase));
        }

        /// <summary>
        /// Simplified constructor.
        /// </summary>
        public WowzaHttpClient(WowzaStreamingCloudConfiguration options)
            : this(options, new Uri($"{options.Host}/api/{options.Version}/")) { }

        /// <summary>
        /// Creates a livestream in the Wowza cloud.
        /// </summary>
        /// <param name="request"><see cref="WscCreateLivestreamRequest"/></param>
        /// <returns><see cref="WscCreateLivestreamResponse"/></returns>
        internal Task<WscCreateLivestreamResponse> CreateLivestreamAsync(WscCreateLivestreamRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            Uri uri = new Uri(ApiBase, "live_streams");
            return SendHttpRequestAsync<WscCreateLivestreamResponse>(HttpMethod.Post, uri, request);
        }

        /// <summary>
        /// Checks if a wowza livestream is streaming at this moment.
        /// </summary>
        /// <remarks>
        /// TODO Make this more bulletproof
        /// </remarks>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="true"/> if streaming</returns>
        public async Task<bool> IsLivestreamStreamingAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            if (!await IsStreamerConnected(id).ConfigureAwait(false)) { return false; }

            var state = await GetLivestreamStateAsync(id).ConfigureAwait(false);
            return state == WscLivestreamState.Started;
        }

        /// <summary>
        /// Starts a livestream in the Wowza cloud.
        /// </summary>
        /// <remarks>
        /// This method can take quite some time, since Wowza needs time to setup
        /// the livestream itself. This function handles all waiting, but can take
        /// up to a few minutes to complete.
        /// </remarks>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StartLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            await TriggerStartLivestreamAsync(id);
            await PollForStateAsync(id, WscLivestreamState.Started);
        }

        /// <summary>
        /// Stops a livestream in the Wowza cloud.
        /// </summary>
        /// <remarks>
        /// This method might take some time, since Wowza might need some time
        /// to close the livestream. This is handled internally.
        /// </remarks>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StopLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            await TriggerStopLivestreamAsync(id);
            await PollForStateAsync(id, WscLivestreamState.Stopped);
        }

        /// <summary>
        /// Start a livestream in the Wowza cloud. Wowza needs time to actually
        /// launch the livestream after this call.
        /// </summary>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="Task"/></returns>
        private Task TriggerStartLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            Uri uri = new Uri(ApiBase, $"live_streams/{id}/start");
            return SendHttpRequestAsync(HttpMethod.Put, uri);
        }

        /// <summary>
        /// Stops a livestream in the Wowza cloud. Wowza sometimes needs time to 
        /// actually stop the livestream after this call.
        /// </summary>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="Task"/></returns>
        private Task TriggerStopLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            Uri uri = new Uri(ApiBase, $"live_streams/{id}/stop");
            return SendHttpRequestAsync(HttpMethod.Put, uri);
        }

        /// <summary>
        /// Keeps on polling the Wowza cloud livestream until it reaches the specified
        /// <paramref name="targetState"/>.
        /// </summary>
        /// <remarks>
        /// Throws a <see cref="TimeoutException"/> if we exceed the timeout.
        /// TODO Semi ugly time management
        /// </remarks>
        /// <param name="livestreamId">Wowza liverstream id</param>
        /// <param name="targetState"><see cref="WscLivestreamState"/></param>
        /// <param name="msInterval">Interval between polls in milliseconds</param>
        /// <param name="msTimeout">Timeout in milliseconds</param>
        /// <returns><see cref="Task"/></returns>
        private async Task PollForStateAsync(string livestreamId, WscLivestreamState targetState, int msInterval = 2000, int msTimeout = 120000)
        {
            livestreamId.ThrowIfNullOrEmpty();

            var msTotal = 0;
            while (await GetLivestreamStateAsync(livestreamId).ConfigureAwait(false) != targetState)
            {
                await Task.Delay(msInterval).ConfigureAwait(false);
                msTotal += msInterval;

                if (msTotal > msTimeout) { throw new TimeoutException("Timed out while polling livestream status"); }
            }
        }

        /// <summary>
        /// Gets the livestream status from the Wowza cloud.
        /// </summary>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="WscLivestreamState"/></returns>
        private async Task<WscLivestreamState> GetLivestreamStateAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            Uri uri = new Uri(ApiBase, $"live_streams/{id}/state");
            var response = await SendHttpRequestAsync<WscLivestreamStatusResponse>(HttpMethod.Get, uri).ConfigureAwait(false);
            if (response == null || response.LiveStream == null) { throw new ArgumentNullException(); }
            return response.LiveStream.State;
        }

        /// <summary>
        /// Checks if a given Wowza livestream has a connected streamer.
        /// </summary>
        /// <param name="id">Wowza vlog id</param>
        /// <returns><see cref="true"/> if connected</returns>
        public async Task<bool> IsStreamerConnected(string id)
        {
            id.ThrowIfNullOrEmpty();
            var uri = new Uri(ApiBase, $"transcoders/{id}/stats");
            var response = await SendHttpRequestAsync<WscTranscoderStats>(HttpMethod.Get, uri).ConfigureAwait(false);
            if (response == null || response.Transcoder == null || response.Transcoder.Connected == null) { throw new ArgumentNullException("Invalid API response object"); }
            return WscEnumParser.Parse(response.Transcoder.Connected.Value);
        }

        /// <summary>
        /// Deletes all outputs for a given transcoder.
        /// </summary>
        /// <param name="transcoderId">Wowza transcoder id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAllOutputsAsync(string transcoderId)
        {
            transcoderId.ThrowIfNullOrEmpty();

            var outputWrapper = await GetOutputsAsync(transcoderId).ConfigureAwait(false);

            foreach (var output in outputWrapper.Outputs)
            {
                var uriOutputDelete = new Uri(ApiBase, $"transcoders/{transcoderId}/outputs/{output.Id}");
                await SendHttpRequestAsync(HttpMethod.Delete, uriOutputDelete).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates a new Fastly stream target in the Wowza API.
        /// </summary>
        /// <returns>Wowza stream target id</returns>
        public async Task<string> CreateFastlyStreamTargetAsync()
        {
            var body = new WscFastlyCreateRequest
            {
                StreamTargetFastly = new SubWscFastlyStreamTarget
                {
                    Name = "Swabbr Fastly Stream Target with SSL and Token Auth",
                    ForceSslPlayback = true,
                    TokenAuthEnabled = true
                }
            };
            var uri = new Uri(ApiBase, "stream_targets/fastly");
            var response = await SendHttpRequestAsync<WscFastlyCreateResponse>(HttpMethod.Post, uri, body).ConfigureAwait(false);
            return response.StreamTargetFastly.Id;
        }

        /// <summary>
        /// Creates a new transcoder output in the Wowza API.
        /// </summary>
        /// <param name="transcoderId">Wowza transcoder id</param>
        /// <returns>Output id</returns>
        public async Task<string> CreateOutputAsync(string transcoderId)
        {
            transcoderId.ThrowIfNullOrEmpty();

            var uri = new Uri(ApiBase, $"transcoders/{transcoderId}/outputs");
            var body = new WscOutputRequest
            {
                Output = new SubWscTranscoderOutputRequest { /* Keep to defaults */ }
            };
            var response = await SendHttpRequestAsync<WscOutputResponse>(HttpMethod.Post, uri, body).ConfigureAwait(false);
            return response.Output.Id;
        }

        /// <summary>
        /// Creates a new stream output target for a given transcoder and links 
        /// it to a Fastly stream target in the Wowza API.
        /// </summary>
        /// <param name="transcoderId">Wowza transcoder id</param>
        /// <param name="outputId">Wowza transcoder output it</param>
        /// <param name="fastlyId">Wowza fastly stream target id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CreateOutputStreamTargetAsync(string transcoderId, string outputId, string fastlyId)
        {
            transcoderId.ThrowIfNullOrEmpty();
            outputId.ThrowIfNullOrEmpty();
            fastlyId.ThrowIfNullOrEmpty();

            var uri = new Uri(ApiBase, $"transcoders/{transcoderId}/outputs/{outputId}/output_stream_targets");
            var body = new WscOutputStreamTargetRequest
            {
                OutputStreamTarget = new SubWscOutputStreamTargetRequest
                {
                    StreamTargetId = fastlyId
                }
            };
            await SendHttpRequestAsync(HttpMethod.Post, uri, body).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the fastly shared secret from the Wowza API.
        /// </summary>
        /// <param name="fastlyId">Wowza Fastly id</param>
        /// <returns>Shared secret</returns>
        public Task<WscFastlyResponse> GetFastlyAsync(string fastlyId)
        {
            fastlyId.ThrowIfNullOrEmpty();
            var uri = new Uri(ApiBase, $"stream_targets/fastly/{fastlyId}");
            return SendHttpRequestAsync<WscFastlyResponse>(HttpMethod.Get, uri);
        }

        /// <summary>
        /// Gets all Wowza outputs, including stream target details, from the
        /// Wowza API.
        /// </summary>
        /// <remarks>
        /// The transcoder id is equal to the live_stream id.
        /// </remarks>
        /// <param name="transcoderId">wowza transcoder id</param>
        /// <returns><see cref="WscOutputsResponse"/></returns>
        public Task<WscOutputsResponse> GetOutputsAsync(string transcoderId)
        {
            transcoderId.ThrowIfNullOrEmpty();
            var uri = new Uri(ApiBase, $"transcoders/{transcoderId}/outputs");
            return SendHttpRequestAsync<WscOutputsResponse>(HttpMethod.Get, uri);
        }

        /// <summary>
        /// Gets a wowza livestream from the Wowza API.
        /// </summary>
        /// <param name="livestreamId">Wowza livestream id</param>
        /// <returns></returns>
        public Task<WscLivestreamResponse> GetLivestreamAsync(string livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            var uri = new Uri(ApiBase, $"live_streams/{livestreamId}");
            return SendHttpRequestAsync<WscLivestreamResponse>(HttpMethod.Get, uri);
        }

        /// <summary>
        /// Called on graceful shutdown, see <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            httpClient.Dispose();
        }


    }
}
