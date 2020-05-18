using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Reprents a single user.
    /// </summary>
    public sealed class UserWithStatsOutputModel : UserOutputModel
    {

        /// <summary>
        /// Total amount of followers the user has acquired.
        /// </summary>
        public int TotalFollowers { get; set; }

        /// <summary>
        /// Total amount of users the subject is following.
        /// </summary>
        public int TotalFollowing { get; set; }

        /// <summary>
        /// Total amount of vlogs posted by the user.
        /// </summary>
        public int TotalVlogs { get; set; }

    }

}
