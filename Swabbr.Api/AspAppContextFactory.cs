using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using System;
using System.Linq;
using System.Security.Claims;

namespace Swabbr.Api;

/// <summary>
///     Factory for creating application context.
/// </summary>
public class AspAppContextFactory : IAppContextFactory
{
    // TODO Protected Get;?
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Create new instance.
    /// </summary>
    public AspAppContextFactory(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    // TODO: Situation where background host is invoked after a http request.
    //       This means we can access a http context, but it won't have a user.
    /// <summary>
    ///     Creates a new instance of an application context.
    /// </summary>
    /// <remarks>
    ///     If no http context can be accessed, an exception is thrown.
    /// </remarks>
    /// <returns>Application context.</returns>
    public Core.AppContext CreateAppContext()
    {
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        if (httpContextAccessor.HttpContext is null)
        {
            // TODO Is this correct?
            // In the case where an ASP context is expected, but we are outside an
            // ASP scope, a different app context factory should be used. Throw.
            throw new InvalidOperationException("Attempting to create ASP app context outside of ASP request scope");
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
