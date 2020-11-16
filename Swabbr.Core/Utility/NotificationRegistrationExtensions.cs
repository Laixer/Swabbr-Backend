using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extensions functionality for the <see cref="NotificationRegistration"/> class.
    /// </summary>
    public static class NotificationRegistrationExtensions
    {

        public static void ThrowIfInvalid(this NotificationRegistration self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            self.Id.ThrowIfNullOrEmpty();
            self.ExternalId.ThrowIfNullOrEmpty();
            self.Handle.ThrowIfNullOrEmpty();
        }

    }

}
