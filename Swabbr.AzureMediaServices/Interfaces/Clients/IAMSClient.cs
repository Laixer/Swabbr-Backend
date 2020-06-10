using Microsoft.Azure.Management.Media.Models;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Interfaces.Clients
{
    /// <summary>
    /// Contract for an Azure Media Services (AMS) handler.
    /// </summary>
    public interface IAMSClient
    {
        /// <summary>
        /// Creates a new <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <returns>Created <see cref="LiveEvent"/></returns>
        Task<LiveEvent> CreateLiveEventAsync(Guid livestreamId);

        /// <summary>
        /// Creates a new <see cref="LiveOutput"/> in AMS.
        /// </summary>
        /// <param name="correspondingVlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns>Created <see cref="LiveOutput"/></returns>
        Task<LiveOutput> CreateLiveOutputAsync(Guid correspondingVlogId, string liveEventName);

        /// <summary>
        /// Creates a new <see cref="StreamingLocator"/> for a <see cref="LiveEvent"/>.
        /// </summary>
        /// <param name="correspondingVlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns>Created <see cref="StreamingLocator"/></returns>
        Task<StreamingLocator> CreateLivestreamVlogStreamingLocatorAsync(Guid correspondingVlogId, string liveEventName);

        /// <summary>
        /// Creates both an input and output <see cref="Asset"/> in AMS for a reaction.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task CreateReactionInputOutputAssetAsync(Guid reactionId);

        /// <summary>
        /// Creates a new <see cref="StreamingLocator"/> for a reaction <see cref="Asset"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task CreateReactionStreamingLocatorAsync(Guid reactionId);

        /// <summary>
        /// Creates a new reaction <see cref="Job"/> in AMS.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <param name="reactionLengthMaxInSeconds">Maximum reaction length after encoding</param>
        /// <returns><see cref="Task"/></returns>
        Task CreateReactionJobAsync(Guid reactionId, uint reactionLengthMaxInSeconds);

        /// <summary>
        /// Deletes an <see cref="Asset"/> in AMS.
        /// </summary>
        /// <param name="assetName">AMs <see cref="Asset"/> name</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteAssetAsync(string assetName);

        /// <summary>
        /// Deletes a reaction <see cref="Job"/> in AMS.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task DeleteReactionJobAsync(Guid reactionId);

        /// <summary>
        /// Ensures that the <see cref="Transform"/> for our <see cref="Core.Entities.Reaction"/>
        /// encoding exists in AMS.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task EnsureReactionTransformExistsAsync();

        /// <summary>
        ///Ensures that the <see cref="Transform"/> for our <see cref="Core.Entities.Livestream"/>
        /// encoding exists in AMS.
        /// TODO Do we even need this?
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task EnsureLivestreamTransformExistsAsync();

        /// <summary>
        /// Ensures that the <see cref="StreamingEndpoint"/> we need for livestreaming is
        /// up and running in AMS.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task EnsureStreamingEndpointRunningAsync();

        /// <summary>
        /// Ensures that the <see cref="ContentKeyPolicy"/> we need for livestreaming is
        /// up and running in AMS.
        /// TODO Do we need this?
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task<ContentKeyPolicy> EnsureContentKeyPolicyExistsAsync();

        /// <summary>
        /// Checks if an <see cref="Asset"/> exists in AMs.
        /// </summary>
        /// <param name="assetName">Name of the <see cref="Asset"/></param>
        /// <returns>Bool result</returns>
        Task<bool> ExistsAssetAsync(string assetName);

        /// <summary>
        /// Creates a new <see cref="LivestreamUpstreamDetails"/> for an AMS livesteam.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="LivestreamUpstreamDetails"/></returns>
        Task<LivestreamUpstreamDetails> GetUpstreamDetailsAsync(Guid livestreamId, Guid vlogId, string liveEventName);

        /// <summary>
        /// Gets the key identifier for a livestream <see cref="StreamingLocator"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <returns>Key identifier</returns>
        Task<string> GetVlogStreamingLocatorKeyIdentifierAsync(Guid livestreamId);

        /// <summary>
        /// Gets a SAS token for a reaction input <see cref="Asset"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        Task<Uri> GetReactionInputAssetSasAsync(Guid reactionId);

        /// <summary>
        /// Gets a SAS token for a reaction output <see cref="Asset"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns>SAS <see cref="Uri"/></returns>
        Task<Uri> GetReactionOutputAssetSasAsync(Guid reactionId);

        /// <summary>
        /// Gets the key identifier for a reaction <see cref="StreamingLocator"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns>Key identifier</returns>
        Task<string> GetReactionStreamingLocatorKeyIdentifierAsync(Guid reactionId);

        /// <summary>
        /// Gets all <see cref="StreamingLocator"/> paths for a vlog.
        /// </summary>
        /// <param name="correspondingVlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <returns><see cref="Uri"/> collection</returns>
        Task<IEnumerable<Uri>> GetVlogStreamingLocatorPathsAsync(Guid correspondingVlogId);

        /// <summary>
        /// Gets all <see cref="StreamingLocator"/> paths for a reaction.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Uri"/> collection</returns>
        Task<IEnumerable<Uri>> GetReactionStreamingLocatorPathsAsync(Guid reactionId);

        /// <summary>
        /// Gets the hostname of our <see cref="StreamingEndpoint"/>.
        /// </summary>
        /// <returns><see cref="Uri"/></returns>
        Task<Uri> GetStreamingEndpointHostNameAsync();

        /// <summary>
        /// Checks if AMS is available.
        /// </summary>
        /// <returns>Bool result</returns>
        Task<bool> IsServiceAvailableAsync();

        /// <summary>
        /// Starts a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        Task StartLiveEventAsync(string liveEventName);

        /// <summary>
        /// Stops a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        Task StopLiveEventAsync(string liveEventName);

        /// <summary>
        /// Stops a <see cref="LiveOutput"/> in AMS.
        /// </summary>
        /// <remarks>
        /// Call this before calling <see cref="StopLiveEventAsync(string)"/>.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        Task StopLiveOutputAsync(Guid vlogId, string liveEventName);
    }
}
