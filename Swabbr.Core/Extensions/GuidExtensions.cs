using System;

namespace Swabbr.Core.Extensions
{
    /// <summary>
    /// Contains extensions for the <see cref="Guid"/> class.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Indicates how long a <see cref="Guid"/> is when parsed to <see cref="string"/>,
        /// according to the 00000000-0000-0000-0000-000000000000 format.
        /// </summary>
        public const int GuidAsStringLength = 36;

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the <paramref name="self"/>
        /// is either null or equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        public static void ThrowIfNullOrEmpty(this Guid self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            if (self == Guid.Empty) { throw new ArgumentNullException("Guid is empty"); }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the <paramref name="self"/>
        /// is not null and not equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        public static void ThrowIfNotNullOrEmpty(this Guid self)
        {
            if (!self.IsNullOrEmpty()) { throw new InvalidOperationException("Guid is not null or empty"); }
        }

        /// <summary>
        /// Checks if a guid is null or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        public static bool IsNullOrEmpty(this Guid self)
        {
            return self == null || self == Guid.Empty;
        }

        /// <summary>
        /// Checks if a guid is not null or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        public static bool IsNotNullOrEmpty(this Guid self)
        {
            return !IsNullOrEmpty(self);
        }
    }
}
