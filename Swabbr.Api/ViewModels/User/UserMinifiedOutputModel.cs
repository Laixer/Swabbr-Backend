using System;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Contains the id and nickname for a user.
    /// </summary>
    public sealed class UserMinifiedOutputModel
    {

        public Guid Id { get; set; }

        public string NickName { get; set; }

    }

}
