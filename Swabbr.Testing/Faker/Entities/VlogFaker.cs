using Bogus;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="Vlog"/> entities.
    /// </summary>
    public class VlogFaker : Faker<Vlog>
    {
        public VlogFaker()
        {
            RuleFor(x => x.DateCreated, x => x.Date.Recent());
            RuleFor(x => x.Id, x => Guid.NewGuid());
            RuleFor(x => x.IsPrivate, x => x.Random.Bool());
            RuleFor(x => x.Length, x => x.Random.UInt(1, 500));
            RuleFor(x => x.ThumbnailUri, new Uri("www.randomuri.com"));
            RuleFor(x => x.UserId, x => Guid.NewGuid());
            RuleFor(x => x.VideoUri, x => new Uri("www.randomuri.com"));
            RuleFor(x => x.Views, x => x.Random.UInt(1, 1000));
            RuleFor(x => x.VlogStatus, x => x.PickRandom<VlogStatus>());
        }
    }
}
