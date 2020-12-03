using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using System;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="FollowRequest"/> entities.
    /// </summary>
    internal static class MapperFollowRequest
    {

        internal static FollowRequestOutputModel Map(FollowRequest followRequest)
        {
            if (followRequest == null) { throw new ArgumentNullException(nameof(followRequest)); }
            return new FollowRequestOutputModel
            {
                ReceiverId = followRequest.Id.ReceiverId,
                RequesterId = followRequest.Id.RequesterId,
                Status = MapperEnum.Map(followRequest.FollowRequestStatus).GetEnumMemberAttribute(),
                DateCreated = followRequest.DateCreated,
                DateUpdated = followRequest.DateUpdated
            };
        }

        internal static FollowRequest Map(FollowRequestOutputModel followRequest) => throw new NotImplementedException();
    }
}
