using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.JsonWrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Streaming service based on Azure Media Services.
    /// </summary>
    public sealed class AMSLivestreamService : ILivestreamService
    {

        private readonly ILivestreamRepository _livestreamRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamService(ILivestreamRepository livestreamRepository)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
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

        public Task<Livestream> TryStartLivestreamForUserAsync(Guid userId, DateTimeOffset triggerMinute)
        {
            throw new NotImplementedException();
        }
    }
}
