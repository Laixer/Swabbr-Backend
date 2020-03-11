using Swabbr.Core.Notifications.JsonWrappers;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// A contract for a service to handle everything regarding notification.
    /// </summary>
    public interface INotificationService
    {

        Task TestNotifationAsync(Guid userId, string message);

        Task VlogRecordRequestAsync(Guid userId, Guid livesteamId, ParametersRecordVlog pars);

        Task NotifyFollowersProfileLiveAsync(Guid userId, Guid livestreamId);

    }

}
