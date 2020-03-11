using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.WowzaStreamingCloud.Client;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Utility;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.WowzaStreamingCloud.Services
{

    /// <summary>
    /// Manages a Wowza <see cref="Livestream"/> pool.
    /// </summary>
    public sealed class WowzaLivestreamPoolService : ILivestreamPoolService
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
                        LivestreamStatus = LivestreamStatus.Created
                    }).ConfigureAwait(false);

                    // Setup the proper outputs in Wowza
                    var transcoderId = livestream.ExternalId;
                    await _wowzaHttpClient.DeleteAllOutputsAsync(transcoderId).ConfigureAwait(false);
                    var outputId = await _wowzaHttpClient.CreateOutputAsync(transcoderId).ConfigureAwait(false);
                    var fastlyId = await _wowzaHttpClient.CreateFastlyStreamTargetAsync().ConfigureAwait(false);
                    await _wowzaHttpClient.CreateOutputStreamTargetAsync(transcoderId, outputId, fastlyId).ConfigureAwait(false);

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
        /// Attempts to get a <see cref="Livestream"/> from the pool.
        /// </summary>
        /// <remarks>
        /// Throws a <see cref="NoLivestreamAvailableException"/> if none is
        /// available.
        /// TODO Implement.
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public Task<Livestream> TryGetLivestreamFromPoolAsync()
        {
            return CreateLivestreamAsync();
        }
    }
}
