using Microsoft.Azure.Management.Media.Models;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Interfaces.Clients
{
    /// <summary>
    /// Contract for an Azure Media Services handler.
    /// </summary>
    public interface IAMSClient
    {
        Task<LiveEvent> CreateLiveEventAsync(Guid livestreamId);

        Task<LiveOutput> CreateLiveOutputAsync(Guid correspondingVlogId, string liveEventName);

        Task CreateReactionStreamingLocatorAsync(Guid reactionId);

        Task<StreamingLocator> CreateLivestreamVlogStreamingLocatorAsync(Guid correspondingVlogId, string liveEventName);

        Task EnsureReactionTransformExistsAsync();

        Task EnsureLivestreamTransformExistsAsync();

        Task EnsureStreamingEndpointRunningAsync();

        Task<ContentKeyPolicy> EnsureContentKeyPolicyExistsAsync();

        Task<LivestreamUpstreamDetails> GetUpstreamDetailsAsync(Guid livestreamId, Guid vlogId, string liveEventName);

        Task<string> GetVlogStreamingLocatorKeyIdentifierAsync(Guid livestreamId);

        Task<string> GetReactionStreamingLocatorKeyIdentifierAsync(Guid livestreamId);

        Task<IEnumerable<string>> GetVlogStreamingLocatorPathsAsync(Guid correspondingVlogId);

        Task<IEnumerable<string>> GetReactionStreamingLocatorPathsAsync(Guid reactionId);

        Task<string> GetStreamingEndpointHostNameAsync();

        Task<bool> IsServiceAvailableAsync();

        Task StartLiveEventAsync(string liveEventName);

        Task StopLiveEventAsync(string liveEventName);

        Task StopLiveOutputAsync(Guid vlogId, string liveEventName);

    }

}
