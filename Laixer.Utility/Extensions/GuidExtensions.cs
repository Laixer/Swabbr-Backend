using System;

namespace Laixer.Utility.Extensions
{

    /// <summary>
    /// Contains extensions for the <see cref="Guid"/> class.
    /// </summary>
    public static class GuidExtensions
    {

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
        /// is not <see cref="null"/> and not equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        public static void ThrowIfNotNullOrEmpty(this Guid self)
        {
            if (!self.IsNullOrEmpty()) { throw new InvalidOperationException("Guid is not null or empty"); }
        }

        /// <summary>
        /// Checks if a <see cref="Guid"/> is <see cref="null"/> or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        /// <returns><see cref="true"/> if null or empty</returns>
        public static bool IsNullOrEmpty(this Guid self)
        {
            return self == null || self == Guid.Empty;
        }

        /// <summary>
        /// Checks if a <see cref="Guid"/> is not <see cref="null"/> or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self"><see cref="Guid"/></param>
        /// <returns><see cref="true"/> if not null or empty</returns>
        public static bool IsNotNullOrEmpty(this Guid self)
        {
            return !IsNullOrEmpty(self);
        }

    }

}
