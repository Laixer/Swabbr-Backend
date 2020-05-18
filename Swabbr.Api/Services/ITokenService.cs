using Swabbr.Api.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Services
{

    /// <summary>
    /// Contract for a token generation service.
    /// </summary>
    public interface ITokenService
    {

        TokenWrapper GenerateToken(SwabbrIdentityUser user);

    }

}
