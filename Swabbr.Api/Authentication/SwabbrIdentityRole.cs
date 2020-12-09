﻿using Microsoft.AspNetCore.Identity;
using System;

namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Represents a Swabbr role.
    /// </summary>
    public class SwabbrIdentityRole : IdentityRole<Guid>
    {
    }
}
