using System;

namespace Swabbr.ViewModels
{
    public class User
    {
        // TODO >...
        public string Id { get; set; }

        public string ProfileImageUrl { get; set; }

        public uint TotalVlogs { get; set; }
        public uint TotalFollowers { get; set; }
        public uint TotalFollowing { get; set; }
    }
}