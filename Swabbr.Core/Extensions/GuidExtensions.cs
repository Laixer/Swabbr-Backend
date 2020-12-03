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
