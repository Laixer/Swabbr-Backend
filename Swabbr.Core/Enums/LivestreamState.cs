using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Indicates the status of a <see cref="Entities.Livestream"/>.
    /// </summary>
    public enum LivestreamState
    {


        /// <summary>
        /// The user never responded.
        /// </summary>
        [EnumMember(Value = "user_no_response_timeout")]
        UserNoResponseTimeout,

        /// <summary>
        /// The livestream is created, but that's all.
        /// </summary>
        [EnumMember(Value = "created_internal")]
        CreatedInternal,

        /// <summary>
        /// The livestream is created, but that's all.
        /// </summary>
        [EnumMember(Value = "created")]
        Created,

        /// <summary>
        /// The user was notified and we are waiting for the user to respond.
        /// </summary>
        [EnumMember(Value = "pending_user")]
        PendingUser,

        /// <summary>
        /// The user is streaming.
        /// </summary>
        [EnumMember(Value = "live")]
        Live,

        /// <summary>
        /// The user stopped streaming and we are waiting for the external service
        /// to close the stream.
        /// </summary>
        [EnumMember(Value = "pending_closure")]
        PendingClosure,

        /// <summary>
        /// The stream is closed.
        /// </summary>
        [EnumMember(Value = "closed")]
        Closed,

        /// <summary>
        /// The user has received the credentials and we are waiting for first connect.
        /// </summary>
        [EnumMember(Value = "pending_user_connect")]
        PendingUserConnect,

        /// <summary>
        /// The user has received the credentials and we are waiting for first connect.
        /// </summary>
        [EnumMember(Value = "user_never_connected_timeout")]
        UserNeverConnectedTimeout,

    }

}
