using Swabbr.Core.Enums;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for managing device & user registration for notifications.
    /// </summary>
    public interface IDeviceRegistrationService
    {

        Task RegisterOnlyThisDeviceAsync(Guid userId, PushNotificationPlatform platform, string handle);

        Task UnregisterAsync(Guid userId);

    }

}
