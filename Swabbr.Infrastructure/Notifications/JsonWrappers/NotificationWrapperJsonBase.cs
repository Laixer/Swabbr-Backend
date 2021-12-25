using Swabbr.Core.Notifications;

namespace Swabbr.Infrastructure.Notifications.JsonWrappers;

/// <summary>
///     Subwrapper to match expected Firebase format.
/// </summary>
internal sealed class SubData
{
    /// <summary>
    ///     Contains the actual notification.
    /// </summary>
    public SwabbrNotification Payload { get; set; }
}

/// <summary>
///     JSON wrapper abstract base class for all notification 
///     objects in Azure Notification Hub.
/// </summary>
internal abstract class NotificationWrapperJsonBase
{
    /// <summary>
    ///     Subwrapper.
    /// </summary>
    public SubData Data { get; set; }
}
