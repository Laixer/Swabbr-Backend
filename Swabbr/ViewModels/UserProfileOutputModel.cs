namespace Swabbr.Api.ViewModels
{
    public class UserProfileOutputModel
    {
        //TODO X
        public string Id { get; set; }

        public string ProfileImageUrl { get; set; }

        public int TotalVlogs { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
    }
}
