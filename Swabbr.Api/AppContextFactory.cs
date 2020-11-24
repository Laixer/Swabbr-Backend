using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Swabbr.Api
{
    /// <summary>
    ///     Factory for creating application context.
    /// </summary>
    public class AppContextFactory : IAppContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AppContextFactory(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <summary>
        ///     Creates a new instance of an application context.
        /// </summary>
        /// <remarks>
        ///     If no http context can be accessed, a new and
        ///     default initialized appcontext is returned.
        /// </remarks>
        /// <returns>Application context.</returns>
        public Core.AppContext CreateAppContext()
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            if (httpContextAccessor is null)
            {
                // Return an empty context if we have no http context.
                return new Core.AppContext();
            }

            var httpContext = httpContextAccessor.HttpContext;
            return new Core.AppContext
            {
                CancellationToken = httpContext.RequestAborted,
                ServiceProvider = httpContext.RequestServices,
                UserId = GetUserIdOrThrow(httpContext.User),
            };
        }

        /// <summary>
        ///     Extracts the user id from the claims principal.
        /// </summary>
        /// <param name="claims">Claims principal.</param>
        /// <returns>The user id.</returns>
        private Guid GetUserIdOrThrow(ClaimsPrincipal claims)
        {
            var idClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim is null)
            {
                throw new InvalidOperationException();
            }

            return Guid.Parse(idClaim.Value);
        }
    }
}
