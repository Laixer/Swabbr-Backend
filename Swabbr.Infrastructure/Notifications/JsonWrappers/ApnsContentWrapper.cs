namespace Swabbr.Infrastructure.Notifications.JsonWrappers;

// FUTURE Implement.
/// <summary>
///     Content wrapper json object template for 
///     Apple Push Notification Services.
/// </summary>
internal sealed class ApnsContentWrapper : NotificationWrapperJsonBase
{
    /// <summary>
    ///     Subwrapper.
    /// </summary>
    public ApnsContentAps Aps { get; set; }
}

/// <summary>
///     Content wrapper json object template 
///     for Apple Push Notification Services.
/// </summary>
internal sealed class ApnsContentAps
{
    /// <summary>
    ///     APNS specific subwrapper.
    /// </summary>
    public string Alert { get; set; }
}
