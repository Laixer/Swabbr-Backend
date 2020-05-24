namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Content wrapper json object template for Firebase Cloud Messaging.
    /// </summary>
    internal sealed class FcmContentWrapper : NotificationJsonBase
    {

        /// <summary>
        /// Firebase specific properties.
        /// TODO These seem optional so let's not use them!
        /// </summary>
        //public FcmContentNotification Notification { get; set; }

        /// <summary>
        /// Custom payload.
        /// TODO Done as JSON wrapper, because this would just send the values.
        /// The keys are lost.
        /// </summary>
        //public NotificationPayloadJsonWrapper Data { get; set; }

        public SubData Data { get; set; }

    }

    internal sealed class SubData
    {
        public SwabbrNotification Payload { get; set; }
    }

}
