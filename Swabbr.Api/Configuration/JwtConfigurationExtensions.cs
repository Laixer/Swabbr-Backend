﻿using Laixer.Utility.Exceptions;
using Laixer.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Configuration
{

    /// <summary>
    /// Contains extension functionality for <see cref="JwtConfiguration"/>.
    /// </summary>
    public static class JwtConfigurationExtensions
    {

        public static void ThrowIfInvalid(this JwtConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (config.ExpireMinutes < 0) { throw new ArgumentOutOfRangeException($"Jwt config {nameof(JwtConfiguration.ExpireMinutes)} can't be negative"); }
            if (config.Issuer.IsNullOrEmpty()) { throw new ConfigurationException($"Jwt config {nameof(JwtConfiguration.Issuer)} can't be null"); }
            if (config.SecretKey.IsNullOrEmpty()) { throw new ConfigurationException($"Jwt config {nameof(JwtConfiguration.SecretKey)} can't be null"); }
        }

    }
}
