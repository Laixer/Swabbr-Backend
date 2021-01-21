using Bogus;
using Bogus.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="UserWithStats"/> entities.
    /// </summary>
    public class UserWithStatsFaker : Faker<UserWithStats>
    {
        public UserWithStatsFaker()
        {
            // TODO Duplicate code with Faker<User>
            RuleFor(x => x.BirthDate, x => x.Date.Recent().OrNull(x, 0.1f));
            RuleFor(x => x.Country, x => x.Random.Replace("???"));
            RuleFor(x => x.DailyVlogRequestLimit, x => x.Random.UInt(0, 3));
            RuleFor(x => x.FirstName, x => x.Person.FirstName);
            RuleFor(x => x.FollowMode, x => x.PickRandom<FollowMode>());
            RuleFor(x => x.Gender, x => x.PickRandom<Gender>());
            RuleFor(x => x.Id, x => Guid.NewGuid());
            RuleFor(x => x.IsPrivate, x => x.Random.Bool());
            RuleFor(x => x.LastName, x => x.Person.LastName);
            RuleFor(x => x.Latitude, x => x.Random.Double(0, 1000));
            RuleFor(x => x.Longitude, x => x.Random.Double(0, 1000));
            RuleFor(x => x.Nickname, x => x.Person.UserName);
            RuleFor(x => x.ProfileImageBase64Encoded, x => x.Random.Replace("########"));
            RuleFor(x => x.TimeZone, x => TimeZoneInfo.Local);

            RuleFor(x => x.TotalFollowers, x => x.Random.UInt(0, 100000));
            RuleFor(x => x.TotalFollowing, x => x.Random.UInt(0, 100));
            RuleFor(x => x.TotalLikesReceived, x => x.Random.UInt(0, 10000));
            RuleFor(x => x.TotalReactionsGiven, x => x.Random.UInt(0, 100));
            RuleFor(x => x.TotalReactionsReceived, x => x.Random.UInt(0, 1000));
            RuleFor(x => x.TotalViews, x => x.Random.UInt(0, 1000000));
            RuleFor(x => x.TotalVlogs, x => x.Random.UInt(0, 25));
        }
    }
}
