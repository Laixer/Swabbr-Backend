using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Contains our user authentication details.
    /// </summary>
    public sealed class UserAuthenticationOutputModel
    {

        /// <summary>
        /// Application access token of the authenticated user.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The time the token is valid.
        /// </summary>
        public TimeSpan TokenExpirationTimespan { get; set; }

        /// <summary>
        /// The moment this token was generated.
        /// </summary>
        public DateTimeOffset TokenCreationDate { get; set; }

        /// <summary>
        /// Claims of the authenticated user.
        /// </summary>
        public IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Application roles of the user.
        /// </summary>
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Information about the authenticated user.
        /// </summary>
        public UserOutputModel User { get; set; }

        /// <summary>
        /// Settings of the authenticated user.
        /// </summary>
        public UserSettingsOutputModel UserSettings { get; set; }

    }

}
