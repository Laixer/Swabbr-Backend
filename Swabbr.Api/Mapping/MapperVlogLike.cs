using Swabbr.Api.ViewModels.VlogLike;
using Swabbr.Core.Entities;
using System;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="VlogLike"/> entities.
    /// </summary>
    internal static class MapperVlogLike
    {

        internal static VlogLikeOutputModel Map(VlogLike vlogLike)
        {
            if (vlogLike == null) { throw new ArgumentNullException(nameof(vlogLike)); }
            return new VlogLikeOutputModel
            {
                UserId = vlogLike.Id.UserId,
                VlogId = vlogLike.Id.VlogId,
                TimeCreated = vlogLike.CreateDate
            };
        }

    }

}
