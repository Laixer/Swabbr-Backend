using System;

namespace Swabbr.Core.Entities
{
    // TODO Better name.
    /// <summary>
    ///     Simple version of a <see cref="SwabbrUser"/>.
    /// </summary>
    /// <remarks>
    ///     This can be used for simple display of users and
    ///     prevents us from having to retrieve all user details.
    /// </remarks>
    public sealed class SwabbrUserSimplified : EntityBase<Guid>
    {
        /// <summary>
        ///     User nickname.
        /// </summary>
        public string Nickname { get; set; }
    }
}
