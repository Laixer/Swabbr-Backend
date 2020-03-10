using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// A contract for a service to handle everything regarding notification.
    /// </summary>
    public interface INotificationService
    {

        Task TestNotifationAsync(Guid userId, string message);

        Task VlogRecordRequestAsync(Guid userId, Guid livesteamId);

        Task NotifyFollowersProfileLiveAsync(Guid userId, Guid livestreamId);

    }

}
