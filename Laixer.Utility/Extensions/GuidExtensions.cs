using System;

namespace Laixer.Utility.Extensions
{

    /// <summary>
    /// Contains extensions for the <see cref="Guid"/> class.
    /// </summary>
    public static class GuidExtensions
    {

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the <paramref name="guid"/>
        /// is either null or equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/></param>
        public static void ThrowIfNullOrEmpty(this Guid guid)
        {
            if (guid == null) { throw new ArgumentNullException(nameof(guid)); }
            if (guid == Guid.Empty) { throw new ArgumentNullException("Guid is empty"); }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the <paramref name="guid"/>
        /// is not <see cref="null"/> and not equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/></param>
        public static void ThrowIfNotNullOrEmpty(this Guid guid)
        {
            if (!guid.IsNullOrEmpty()) { throw new InvalidOperationException("Guid is not null or empty"); }
        }

        /// <summary>
        /// Checks if a <see cref="Guid"/> is <see cref="null"/> or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/></param>
        /// <returns><see cref="true"/> if null or empty</returns>
        public static bool IsNullOrEmpty(this Guid guid)
        {
            return guid == null || guid == Guid.Empty;
        }

        /// <summary>
        /// Checks if a <see cref="Guid"/> is not <see cref="null"/> or <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/></param>
        /// <returns><see cref="true"/> if not null or empty</returns>
        public static bool IsNotNullOrEmpty(this Guid guid)
        {
            return !IsNullOrEmpty(guid);
        }

    }

}
