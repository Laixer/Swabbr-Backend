namespace Swabbr.Api.ViewModels
{
    public class UserStatisticsOutputModel
    {
        public int TotalLikes { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
        public int TotalReactionsGiven { get; set; }
        public int TotalReactionsReceived { get; set; }
        public int TotalVlogs { get; set; }
        public int TotalViews { get; set; }
    }
}