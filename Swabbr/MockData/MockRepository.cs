using Swabbr.Api.ViewModels;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static UserOutputModel RandomUserOutputMock(Guid id = default)
        {
            return new UserOutputModel
            {
                UserId = id,
                Email = $"{RandomString(3)}@{RandomString(4)}.{RandomString(3)}",
                FirstName = RandomString(2),
                LastName = RandomString(10),
                Nickname = RandomString(4),
                BirthDate = DateTime.Now,
                Country = RandomString(2),
                Gender = RandomString(26).Contains("A", StringComparison.CurrentCultureIgnoreCase) ? Gender.Female : Gender.Male
            };
        }

        public static UserProfileOutputModel RandomUserProfileOutput(Guid id = default)
        {
            return new UserProfileOutputModel
            {
                UserId = id,
                FirstName = RandomString(2),
                LastName = RandomString(10),
                Nickname = RandomString(4),
                BirthDate = DateTime.Now,
                Country = RandomString(2),
                Gender = RandomString(26).Contains("A", StringComparison.CurrentCultureIgnoreCase) ? Core.Enums.Gender.Female : Gender.Male
            };
        }

        public static FollowRequestOutputModel RandomFollowRequestOutput()
        {
            return new FollowRequestOutputModel
            {
                FollowRequestId = Guid.NewGuid(),
                ReceiverId = Guid.NewGuid(),
                RequesterId = Guid.NewGuid(),
                Status = FollowRequestStatus.Pending,
                TimeCreated = DateTime.Now
            };
        }

        public static UserSettingsOutputModel RandomUserSettingsOutput()
        {
            return new UserSettingsOutputModel
            {
                UserId = Guid.NewGuid(),
                DailyVlogRequestLimit = 2,
                FollowMode = FollowMode.AcceptAll,
                IsPrivate = true
            };
        }

        public static VlogOutputModel RandomVlogOutput(Guid id = default)
        {
            var vlogId = Guid.NewGuid();

            return new VlogOutputModel
            {
                UserId = new Guid("88e04067-b486-41e3-83b0-0b5e5ca1e9a5"),
                VlogId = vlogId,
                IsLive = true,
                IsPrivate = false,
                DateStarted = DateTime.Now.Subtract(TimeSpan.FromHours(2)),
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

        #endregion I/O models
    }
}