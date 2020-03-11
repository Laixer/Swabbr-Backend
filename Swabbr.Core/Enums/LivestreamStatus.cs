using NpgsqlTypes;
using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Indicates the status of a <see cref="Entities.Livestream"/>.
    /// </summary>
    public enum LivestreamStatus
    {

        /// <summary>
        /// The livestream is created, but that's all.
        /// </summary>
        [EnumMember(Value = "created")]
        [PgName("created")]
        Created,

        /// <summary>
        /// The user was notified and we are waiting for the user to respond.
        /// </summary>
        [EnumMember(Value = "pending_user")]
        [PgName("pending_user")]
        PendingUser,

        /// <summary>
        /// The user is streaming.
        /// </summary>
        [EnumMember(Value = "live")]
        [PgName("live")]
        Live,

        /// <summary>
        /// The user stopped streaming and we are waiting for the external service
        /// to close the stream.
        /// </summary>
        [EnumMember(Value = "pending_closure")]
        [PgName("pending_closure")]
        PendingClosure,

        /// <summary>
        /// The stream is closed.
        /// </summary>
        [EnumMember(Value = "closed")]
        [PgName("closed")]
        Closed,

        /// <summary>
        /// The user never responded.
        /// </summary>
        [EnumMember(Value = "user_no_response_timeout")]
        [PgName("user_no_response_timeout")]
        UserNoResponseTimeout,

    }

}
