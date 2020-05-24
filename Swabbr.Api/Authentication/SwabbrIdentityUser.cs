using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Identity;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.Authentication
{

    /// <summary>
    /// Custom Identity framework user.
    /// </summary>
    public class SwabbrIdentityUser : IdentityUser<Guid>
    {

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

    }

}
