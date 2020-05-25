using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{

    /// <summary>
    /// Wrapper around an access token and its metadata.
    /// </summary>
    public sealed class TokenWrapper
    {

        /// <summary>
        /// The acces token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The <see cref="DateTimeOffset"/> at which the access token was created.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// <see cref="TimeSpan"/> indicating how long the access token is valid.
        /// </summary>
        public TimeSpan TokenExpirationTimespan { get; set; }

    }

}
