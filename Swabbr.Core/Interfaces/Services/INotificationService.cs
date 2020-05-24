using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// A contract for a service to handle everything regarding notification.
    /// </summary>
    public interface INotificationService
    {

        Task NotifyVlogRecordRequestAsync(Guid userId, Guid livesteamId, ParametersRecordVlog pars);

        Task NotifyVlogRecordTimeoutAsync(Guid userId);

        Task NotifyFollowersProfileLiveAsync(Guid userId, Guid livestreamId, ParametersFollowedProfileLive pars);

        Task NotifyFollowersVlogPostedAsync(Guid userId, Guid vlogId);

        Task NotifyReactionPlacedAsync(Guid reactionId);

        Task NotifyVlogLikedAsync(VlogLikeId vlogLikeId);

    }

}
