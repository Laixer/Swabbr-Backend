using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    public interface INotificationTestingService
    {

        Task NotifyVlogRecordRequestAsync(Guid userId);

        Task NotifyVlogRecordTimeoutAsync(Guid userId);

        Task NotifyFollowersProfileLiveAsync(Guid userId);

        Task NotifyFollowersVlogPostedAsync(Guid userId);

        Task NotifyReactionPlacedAsync(Guid userId);

        Task NotifyVlogLikedAsync(Guid userId);

    }
}
