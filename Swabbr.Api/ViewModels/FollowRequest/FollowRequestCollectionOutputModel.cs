using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.FollowRequest
{

    public sealed class FollowRequestCollectionOutputModel
    {

        public IEnumerable<FollowRequestOutputModel> FollowRequests { get; set; }

        public int Count => FollowRequests != null ? FollowRequests.Count() : 0;

    }

}
