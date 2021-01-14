using Bogus;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="FollowRequest"/> entities.
    /// </summary>
    public class FollowRequestFaker : Faker<FollowRequest>
    {
        public FollowRequestFaker()
        {
            RuleFor(x => x.DateCreated, x => x.Date.Recent());
            RuleFor(x => x.DateUpdated, x => x.Date.Recent());
            RuleFor(x => x.FollowRequestStatus, x => x.PickRandom<FollowRequestStatus>());
            RuleFor(x => x.Id, x => new FollowRequestId
            {
                ReceiverId = Guid.NewGuid(),
                RequesterId = Guid.NewGuid(),
            });
        }
    }
}
