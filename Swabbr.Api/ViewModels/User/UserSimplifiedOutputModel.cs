using System;

namespace Swabbr.Api.ViewModels.User
{
    /// <summary>
    /// Contains the id and nickname for a user.
    /// </summary>
    public sealed class UserSimplifiedOutputModel
    {
        /// <summary>
        ///     User id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     User nickname.
        /// </summary>
        public string NickName { get; set; }
    }
}
