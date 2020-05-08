using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.FollowRequest
{

    /// <summary>
    /// Output model for repesenting a <see cref="Core.Entities.FollowRequest"/>
    /// <see cref="Core.Enums.FollowRequestStatus"/>.
    /// </summary>
    public sealed class FollowRequestStatusOutputModel
    {

        public string Status { get; set; }

    }

}
