using Bogus;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="VlogLike"/> entities.
    /// </summary>
    public class VlogLikeFaker : Faker<VlogLike>
    {
        public VlogLikeFaker()
        {
            RuleFor(x => x.DateCreated, x => x.Date.Recent());
            RuleFor(x => x.Id, x => new VlogLikeId
            {
                UserId = Guid.NewGuid(),
                VlogId = Guid.NewGuid(),
            });
        }
    }
}
