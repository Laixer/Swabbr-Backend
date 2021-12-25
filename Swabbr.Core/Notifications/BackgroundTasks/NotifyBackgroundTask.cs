using Microsoft.Extensions.Logging;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications.Data;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Notifications.BackgroundTasks;

/// <summary>
///     Background task notifying a single user.
/// </summary>
/// <typeparam name="TData">The notification data type.</typeparam>
public class NotifyBackgroundTask<TData> : BackgroundTask
    where TData : NotificationData
{
    /// <summary>
    ///     Protected notification client.
    /// </summary>
    /// <remarks>
    ///     All notification background tasks that inherit
    ///     from this class will most likely use this, hence
    ///     the design choice to make it protected.
    /// </remarks>
    protected INotificationClient _notificationClient;

    private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<NotifyBackgroundTask<TData>> _logger;

    /// <summary>
    ///     Create new instance.
    /// </summary>
    public NotifyBackgroundTask(INotificationClient notificationClient,
        INotificationRegistrationRepository notificationRegistrationRepository,
        ILogger<NotifyBackgroundTask<TData>> logger,
        IUserRepository userRepository)
    {
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    ///     Checks if we can handle the context value object.
    /// </summary>
    /// <param name="value">The object to check.</param>
    public override bool CanHandle(object value)
        => value is NotificationContext c
            && c.HasUser
            && c.Notification is not null
            && c.Notification.Data is TData;

    /// <summary>
    ///     Notify a single user.
    /// </summary>
    /// <param name="context">The background context.</param>
    public override async Task ExecuteAsync(BackgroundTaskContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var notificationContext = (NotificationContext)context.Value;

        // Check if we can reach the user that should be notified.
        if (!await _notificationRegistrationRepository.ExistsAsync(notificationContext.NotifiedUserId))
        {
            _logger.LogTrace($"Couldn't get notification registration for user {notificationContext.NotifiedUserId}");
            return;
        }

        var pushDetails = await _userRepository.GetPushDetailsAsync(notificationContext.NotifiedUserId);
        await _notificationClient.SendNotificationAsync(pushDetails, notificationContext.Notification);

        _logger.LogTrace($"Sent notification to user {notificationContext.NotifiedUserId}");
    }
}