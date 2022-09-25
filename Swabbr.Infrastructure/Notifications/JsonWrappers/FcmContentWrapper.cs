namespace Swabbr.Infrastructure.Notifications.JsonWrappers;

/// <summary>
///     JSON wrapper template for Firebase Cloud Messaging.
/// </summary>
internal sealed class FcmContentWrapper : NotificationWrapperJsonBase
{
    /// <summary>
    ///     Subwrapper.
    /// </summary>
    public FcmContentNotification Notification { get; set; }
}

/// <summary>
///     Content wrapper json object template 
///     for Apple Push Notification Services.
/// </summary>
internal sealed class FcmContentNotification
{
    /// <summary>
    ///     APNS specific title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     APNS specific body.
    /// </summary>
    public string Body { get; set; }
}

