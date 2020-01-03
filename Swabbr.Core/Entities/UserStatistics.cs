using System;

namespace Swabbr.Core.Entities
{
    public class UserStatistics
    {
        public Guid UserId { get; set; }
        public int TotalLikes { get; set; }
        public int TotalReactionsGiven { get; set; }
        public int TotalReactionsReceived { get; set; }
        public int TotalVlogs { get; set; }
        public int TotalViews { get; set; }
    }
}