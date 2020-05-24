using System.Runtime.Serialization;

namespace Swabbr.Api.ViewModels.Enums
{

    /// <summary>
    /// Represents a notification click action.
    /// </summary>
    public enum NotificationActionModel
    {

        [EnumMember(Value = "followed_profile_live")]
        FollowedProfileLive,

        [EnumMember(Value = "followed_profile_vlog_posted")]
        FollowedProfileVlogPosted,

        [EnumMember(Value = "inactive_user_motivate")]
        InactiveUserMotivate,

        [EnumMember(Value = "inactive_unwatched_vlogs")]
        InactiveUnwatchedVlogs,

        [EnumMember(Value = "inactive_vlog_record_request")]
        InactiveVlogRecordRequest,

        [EnumMember(Value = "vlog_gained_likes")]
        VlogGainedLikes,

        [EnumMember(Value = "vlog_new_reaction")]
        VlogNewReaction,

        [EnumMember(Value = "vlog_record_request")]
        VlogRecordRequest

    }

}
