using Swabbr.Api.ViewModels.Enums;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for our enums.
    /// </summary>
    internal static class MapperEnum
    {

        internal static Gender Map(GenderModel gender)
        {
            switch (gender)
            {
                case GenderModel.Female:
                    return Gender.Female;
                case GenderModel.Male:
                    return Gender.Male;
                case GenderModel.Unspecified:
                    return Gender.Unspecified;
            }

            throw new InvalidOperationException(nameof(gender));
        }

        internal static GenderModel Map(Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return GenderModel.Female;
                case Gender.Male:
                    return GenderModel.Male;
                case Gender.Unspecified:
                    return GenderModel.Unspecified;
            }

            throw new InvalidOperationException(nameof(gender));
        }

        internal static FollowMode Map(FollowModeModel followMode)
        {
            switch (followMode)
            {
                case FollowModeModel.Manual:
                    return FollowMode.Manual;
                case FollowModeModel.AcceptAll:
                    return FollowMode.AcceptAll;
                case FollowModeModel.DeclineAll:
                    return FollowMode.DeclineAll;
            }

            throw new InvalidOperationException(nameof(followMode));
        }

        internal static FollowModeModel Map(FollowMode followMode)
        {
            switch (followMode)
            {
                case FollowMode.Manual:
                    return FollowModeModel.Manual;
                case FollowMode.AcceptAll:
                    return FollowModeModel.AcceptAll;
                case FollowMode.DeclineAll:
                    return FollowModeModel.DeclineAll;
            }

            throw new InvalidOperationException(nameof(followMode));
        }

        internal static PushNotificationPlatform Map(PushNotificationPlatformModel platform)
        {
            switch (platform)
            {
                case PushNotificationPlatformModel.APNS:
                    return PushNotificationPlatform.APNS;
                case PushNotificationPlatformModel.FCM:
                    return PushNotificationPlatform.FCM;
            }

            throw new InvalidOperationException(nameof(platform));
        }

        internal static PushNotificationPlatformModel Map(PushNotificationPlatform platform)
        {
            switch (platform)
            {
                case PushNotificationPlatform.APNS:
                    return PushNotificationPlatformModel.APNS;
                case PushNotificationPlatform.FCM:
                    return PushNotificationPlatformModel.FCM;
            }

            throw new InvalidOperationException(nameof(platform));
        }

        internal static FollowRequestStatusModel Map(FollowRequestStatus status)
        {
            switch (status)
            {
                case FollowRequestStatus.Pending:
                    return FollowRequestStatusModel.Pending;
                case FollowRequestStatus.Accepted:
                    return FollowRequestStatusModel.Accepted;
                case FollowRequestStatus.Declined:
                    return FollowRequestStatusModel.Declined;
            }

            throw new InvalidOperationException(nameof(status));
        }

        internal static FollowRequestStatus Map(FollowRequestStatusModel status)
        {
            switch (status)
            {
                case FollowRequestStatusModel.Pending:
                    return FollowRequestStatus.Pending;
                case FollowRequestStatusModel.Accepted:
                    return FollowRequestStatus.Accepted;
                case FollowRequestStatusModel.Declined:
                    return FollowRequestStatus.Declined;
            }

            throw new InvalidOperationException(nameof(status));
        }

    }
}
