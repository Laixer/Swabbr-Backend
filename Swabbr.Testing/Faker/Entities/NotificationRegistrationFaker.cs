using Bogus;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Testing.Faker.Entities
{
    /// <summary>
    ///     Faker for <see cref="NotificationRegistration"/> entities.
    /// </summary>
    public class NotificationRegistrationFaker : Faker<NotificationRegistration>
    {
        public NotificationRegistrationFaker()
        {
            RuleFor(x => x.ExternalId, x => x.Random.Hash(32));
            RuleFor(x => x.Handle, x => x.Random.Hash(32));
            RuleFor(x => x.Id, x => Guid.NewGuid());
            RuleFor(x => x.PushNotificationPlatform, x => x.PickRandom<PushNotificationPlatform>());
        }
    }
}
