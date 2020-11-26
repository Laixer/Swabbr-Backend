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
            if (httpContextAccessor.HttpContext?.User is null)
            {
                // This would be undefined behaviour.
                throw new InvalidOperationException();
            }

            var httpContext = httpContextAccessor.HttpContext;
            return new Core.AppContext
            {
                CancellationToken = httpContext.RequestAborted,
                ServiceProvider = httpContext.RequestServices,
                UserId = IsSignedIn(httpContext.User) ? GetUserIdOrThrow(httpContext.User) : Guid.Empty
            };
        }

        /// <summary>
        ///     Extracts the user id from the claims principal.
        /// </summary>
        /// <param name="principal">Claims principal.</param>
        /// <returns>The user id.</returns>
        private static Guid GetUserIdOrThrow(ClaimsPrincipal principal)
        {
            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim is null)
            {
                throw new InvalidOperationException();
            }

            return Guid.Parse(idClaim.Value);
        }

        /// <summary>
        ///     Returns true if we have a user signed in.
        /// </summary>
        /// <param name="principal">The claims principal.</param>
        private static bool IsSignedIn(ClaimsPrincipal principal)
            => principal?.Identities != null && principal.Identities.Any(i => i.IsAuthenticated);
    }
}
