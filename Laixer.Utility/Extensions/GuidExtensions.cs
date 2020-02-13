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

    }

}
