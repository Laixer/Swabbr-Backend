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

        public string Token { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public TimeSpan TokenExpirationTimespan { get; set; }

    }

}
