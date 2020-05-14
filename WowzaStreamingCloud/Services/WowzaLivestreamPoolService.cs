using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Entities.Outputs;
using Swabbr.WowzaStreamingCloud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Manages a Wowza <see cref="Livestream"/> pool.
    /// </summary>
    public sealed class WowzaLivestreamPoolService
    {

        // TODO DRY client
        private readonly WowzaHttpClient _wowzaHttpClient;
        private readonly WowzaStreamingCloudConfiguration _wscOptions;
        private readonly ILivestreamRepository _livestreamRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public WowzaLivestreamPoolService(ILivestreamRepository livestreamRepository,
            IOptions<WowzaStreamingCloudConfiguration> wscOptions)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            if (wscOptions == null || wscOptions.Value == null) { throw new ArgumentNullException(nameof(wscOptions)); }
            _wscOptions = wscOptions.Value;
            _wowzaHttpClient = new WowzaHttpClient(_wscOptions);
        }

        public Task CleanupLivestreamAsync()
        {
            throw new NotImplementedException();
        }

        public Task CleanupLivestreamAsync(Guid livestreamId)
        {
            throw new NotImplementedException();
        }

        public Task CleanupTimedOutLivestreamAsync(Guid livestreamId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates and sets up a new <see cref="Livestream"/> in the Wowza
        /// platform. This also commits said livestream to our data store.
        /// </summary>
        /// <remarks>
        /// TODO Put all other Wowza-specific properties in a json file here to store in the db!
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> CreateLivestreamAsync()
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // First create a livestream
                    var response = await _wowzaHttpClient.CreateLivestreamAsync(RequestBodyGenerator.CreateLivestream(_wscOptions)).ConfigureAwait(false);
                    var livestream = await _livestreamRepository.CreateAsync(new Livestream
                    {
                        ExternalId = response.Livestream.Id,
                        IsActive = false,
                        BroadcastLocation = response.Livestream.BroadcastLocation,
                        CreateDate = response.Livestream.CreatedAt,
                        Name = response.Livestream.Name,
                        LivestreamState = LivestreamState.Created
                    }).ConfigureAwait(false);

                    // Grab the passthrough output and enable auth
                    var outputs = await _wowzaHttpClient.GetOutputsAsync(livestream.ExternalId).ConfigureAwait(false);

                    // TODO For some reason this didn't work with Linq
                    var matchingOutputs = new List<SubWscOutput>();
                    foreach (var x in outputs.Outputs)
                    {
                        if (x.PassthroughVideo == WowzaConstants.OutputPassthroughVideo &&
                            x.AspectRatioWidth == WowzaConstants.OutputAspectRatioWidth &&
                            x.AspectRatioHeight == WowzaConstants.OutputAspectRatioHeight &&
                            x.OutputStreamTargets.Count() == 1 &&
                            x.OutputStreamTargets.First().StreamTarget.Type.Equals(WowzaConstants.StreamTargetType))
                        {
                            matchingOutputs.Add(x);
                        }
                    }
                    if (!matchingOutputs.Any()) { throw new InvalidOperationException("Could not find correct Wowza Output (passthrough video, 1280x720)"); }
                    if (matchingOutputs.Count() > 1) { throw new InvalidOperationException("Found more than one correct Wowza Output (passthrough video, 1280x720)"); }

                    // Setup authentication for the selected output
                    var fastlyId = matchingOutputs.First().OutputStreamTargets.First().StreamTargetId;
                    await _wowzaHttpClient.SetupFastlyAuthenticationAsync(fastlyId).ConfigureAwait(false);

                    scope.Complete();
                    return livestream;
                }
            }
            catch (Exception e)
            {
                // TODO Do we want this? Like this we can't db-retrieve our livestream after it was created externally
                throw new ExternalErrorException(e.Message);
            }
        }

        /// <summary>
        /// At the moment this just returns <see cref="CreateLivestreamAsync"/>.
        /// </summary>
        /// <returns><see cref="Livestream"/></returns>
        public Task<Livestream> TryGetLivestreamFromPoolAsync()
        {
            return CreateLivestreamAsync();
        }

    }

}
