using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.WowzaStreamingCloud
{
    public sealed class WowzaHmacToken
    {

        public string JwtToken { get; set; }

        public string Hmac { get; set; }

        public DateTimeOffset Expires { get; set; }

        public long ExpiresEpoch => Expires.ToUnixTimeSeconds();

    }
}
