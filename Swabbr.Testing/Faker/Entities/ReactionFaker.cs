using Bogus;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="Reaction"/> entities.
    /// </summary>
    public class ReactionFaker : Faker<Reaction>
    {
        public ReactionFaker()
        {
            RuleFor(x => x.DateCreated, x => x.Date.Recent());
            RuleFor(x => x.Id, x => Guid.NewGuid());
            RuleFor(x => x.IsPrivate, x => x.Random.Bool());
            RuleFor(x => x.Length, x => x.Random.UInt(1, 500));
            RuleFor(x => x.ReactionStatus, x => x.PickRandom<ReactionStatus>());
            RuleFor(x => x.TargetVlogId, x => Guid.NewGuid());
            RuleFor(x => x.ThumbnailUri, new Uri("www.randomuri.com"));
            RuleFor(x => x.UserId, x => Guid.NewGuid());
            RuleFor(x => x.VideoUri, x => new Uri("www.randomuri.com"));
        }
    }
}
