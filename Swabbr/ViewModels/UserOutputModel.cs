using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels
{
    public class UserOutputModel
    {
        // TODO >...
        public string Id { get; set; }

        public string ProfileImageUrl { get; set; }

        public int TotalVlogs { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
    }
}
