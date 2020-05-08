using Newtonsoft.Json;
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
        /// The amount of seconds the token is valid.
        /// TODO Implement
        /// </summary>
        public int TokenValid { get; set; }

        /// <summary>
        /// Claims of the authenticated user.
        /// </summary>
        public IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Application roles of the user.
        /// TODO This can be hard typed
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
