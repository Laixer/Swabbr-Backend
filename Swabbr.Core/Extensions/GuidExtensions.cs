using System;

namespace Swabbr.Core.Extensions
{
    // TODO Remove?
    /// <summary>
    ///     Contains extensions for the <see cref="Guid"/> class.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        ///     Indicates how long a <see cref="Guid"/> is when parsed to <see cref="string"/>,
        ///     according to the 00000000-0000-0000-0000-000000000000 format.
        /// </summary>
        public const int GuidAsStringLength = 36;

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/>
        ///     if the guid is equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self">The guid to check.</param>
        public static void ThrowIfEmpty(this Guid self)
        {
            if (IsEmpty(self)) 
            {
                throw new ArgumentNullException("Guid is empty"); 
            }
        }

        /// <summary>
        ///     Checks if a guid is equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="self">The guid to check.</param>
        public static bool IsEmpty(this Guid self) 
            => self == Guid.Empty;
    }
}
