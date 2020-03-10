using Laixer.Utility.Extensions;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities;
using Swabbr.WowzaStreamingCloud.Enums;
using Swabbr.WowzaStreamingCloud.Parsing;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
        internal Task<WscCreateLivestreamResponse> CreateLivestream(WscCreateLivestreamRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            Uri uri = new Uri(ApiBase, "live_streams");
            return SendHttpRequestAsync<WscCreateLivestreamResponse>(HttpMethod.Post, uri, request);
        }

        /// <summary>
        /// Deletes a livestream from the Wowza cloud.
        /// </summary>
        /// <param name="id">Wowza livesteam id</param>
        /// <returns><see cref="Task"/></returns>
        internal Task DeleteLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            // TODO First maybe get the item and check for edge cases

            Uri uri = new Uri(ApiBase, $"live_streams/{id}");
            return SendHttpRequestAsync(HttpMethod.Delete, uri);
        }

        /// <summary>
        /// Gets a livestream object from the Wowza cloud.
        /// </summary>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="WscCreateLivestreamResponse"/></returns>
        public Task<WscGetLivestreamResponse> GetLivestreamAsync(string id)
        {
            id.ThrowIfNullOrEmpty();

            Uri uri = new Uri(ApiBase, $"live_streams/{id}");
            return SendHttpRequestAsync<WscGetLivestreamResponse>(HttpMethod.Get, uri);
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
            var response = await SendHttpRequestAsync<WscGetLivestreamStatusResponse>(HttpMethod.Get, uri).ConfigureAwait(false);
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
        /// Checks if a given Wowza livestream has a stored recording. In this way
        /// we can verify if data has already been streamed to a livestream.
        /// </summary>
        /// <remarks>
        /// TODO Is this a race condition with the wowza server?
        /// </remarks>
        /// <param name="id">Wowza livestream id</param>
        /// <returns><see cref="true"/> if it has a recording</returns>
        public async Task<bool> DoesLivestreamHaveRecording(string id)
        {
            id.ThrowIfNullOrEmpty();

            var uri = new Uri(ApiBase, $"transcoders/{id}/recordings");
            var response = await SendHttpRequestAsync<WscTranscoderRecordings>(HttpMethod.Get, uri).ConfigureAwait(false);
            if (response == null || response.Recordings == null) { throw new ArgumentNullException("Invalid API response object"); }
            if (response.Recordings.Length == 0) { return false; }
            else if (response.Recordings.Length == 1) { return true; }
            else { throw new InvalidOperationException("Multiple recordings found for single livestream"); }
        }




        /* TO BE REFACTORED */


        public async Task<WscGetRecordingDetailsResponse> GetRecordingsAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"transcoders/{id}/recordings");
            return await SendHttpRequestAsync<WscGetRecordingDetailsResponse>(HttpMethod.Get, uri);
        }

        public async Task<WscGetThumbnailResponse> GetThumbnailUrlAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}/thumbnail_url");
            return await SendHttpRequestAsync<WscGetThumbnailResponse>(HttpMethod.Get, uri);
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
