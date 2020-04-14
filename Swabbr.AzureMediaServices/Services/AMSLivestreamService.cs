using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Streaming service based on Azure Media Services.
    /// </summary>
    public sealed class AMSLivestreamService : ILivestreamService
    {

        private readonly ILivestreamRepository _livestreamRepository;
        private readonly ILivestreamPoolService _livestreamPoolService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamService(ILivestreamRepository livestreamRepository,
            ILivestreamPoolService livestreamPoolService)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _livestreamPoolService = livestreamPoolService ?? throw new ArgumentNullException(nameof(livestreamPoolService));
        }

        public Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute)
        {
            throw new NotImplementedException();
        }

        public Task<ParametersRecordVlog> GetUpstreamParametersAsync(Guid livestreamId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsLivestreamValidForFollowersAsync(Guid livestreamId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task OnUserStartStreamingAsync(Guid livestreamId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task ProcessTimeoutAsync(Guid userId, Guid livestreamId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to setup and start a <see cref="Livestream"/> for a 
        /// <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="triggerMinute">Trigger minute</param>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> TryStartLivestreamForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            userId.ThrowIfNullOrEmpty();
            if (triggerMinute == null) { throw new ArgumentNullException(nameof(triggerMinute)); }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestream = await _livestreamPoolService.TryGetLivestreamFromPoolAsync().ConfigureAwait(false);
                await StartLivestreamAsync(livestream.Id).ConfigureAwait(false);
                await _livestreamRepository.MarkPendingUserAsync(livestream.Id, userId, triggerMinute).ConfigureAwait(false);
                scope.Complete();
                return livestream;
            }
        }

        /// <summary>
        /// Starts a <see cref="Livestream"/> in AMS.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Task"/></returns>
        private async Task StartLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            throw new NotImplementedException();
        }

    }
}
