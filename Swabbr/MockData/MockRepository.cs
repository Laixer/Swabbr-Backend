using Swabbr.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.MockData
{
    //TODO Remove
    public static class MockRepository
    {
        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #region I/O models
        public static UserOutputModel RandomUserOutputMock()
        {
            return new UserOutputModel
            {
                UserId = Guid.NewGuid(),
                Email = $"{RandomString(3)}@{RandomString(4)}.{RandomString(3)}",
                FirstName = RandomString(2),
                LastName = RandomString(10),
                Nickname = RandomString(4),
                BirthDate = DateTime.Now,
                Country = RandomString(2),
                Gender = RandomString(26).Contains("A", StringComparison.CurrentCultureIgnoreCase) ? Core.Enums.Gender.Female : Core.Enums.Gender.Male
            };
        }

        public static UserProfileOutputModel RandomUserProfileOutput()
        {
            return new UserProfileOutputModel
            {
                UserId = Guid.NewGuid(),
                FirstName = RandomString(2),
                LastName = RandomString(10),
                Nickname = RandomString(4),
                BirthDate = DateTime.Now,
                Country = RandomString(2),
                Gender = RandomString(26).Contains("A", StringComparison.CurrentCultureIgnoreCase) ? Core.Enums.Gender.Female : Core.Enums.Gender.Male
            };
        }

        public static FollowRequestOutputModel RandomFollowRequestOutput()
        {
            return new FollowRequestOutputModel
            {
                FollowRequestId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid(),
                RequesterId = Guid.NewGuid(),
                Status = Core.Enums.FollowRequestStatus.Pending,
                TimeCreated = DateTime.Now
            };
        }

        public static UserSettingsOutputModel RandomUserSettingsOutput()
        {
            return new UserSettingsOutputModel
            {
                UserId = Guid.NewGuid(),
                DailyVlogRequestLimit = 2,
                FollowMode = Core.Enums.FollowMode.AcceptAll,
                IsPrivate = true
            };
        }

        public static VlogOutputModel RandomVlogOutput()
        {
            var vlogId = Guid.NewGuid();

            return new VlogOutputModel
            {
                UserId = Guid.NewGuid(),
                VlogId = vlogId,
                IsLive = true,
                IsPrivate = false,
                Likes = new List<Core.Entities.VlogLike>
                {
                    new Core.Entities.VlogLike
                    {
                        VlogLikeId = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        VlogId = vlogId,
                        TimeCreated = DateTime.Now.Subtract(TimeSpan.FromHours(1))
                    }
                }
            };
        }
        #endregion
    }
}
