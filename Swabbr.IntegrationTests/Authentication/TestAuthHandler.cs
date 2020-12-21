using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Swabbr.IntegrationTests.Authentication
{
    /// <summary>
    ///     Authentication handler for testing scenarios.
    /// </summary>
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "Test";

        public TestAuthenticationSchemeOptions SchemeOptions { get; set; }

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            TestAuthenticationSchemeOptions authPrincipal)
            : base(options, logger, encoder, clock) 
            => SchemeOptions = authPrincipal;

        /// <summary>
        ///     Handles an authentication request.
        /// </summary>
        /// <returns>Authentication result.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, SchemeOptions.User.Id.ToString()),
            };
            var identity = new ClaimsIdentity(claims, AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

            var claimsPrincipal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(claimsPrincipal, AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties) 
            => base.HandleForbiddenAsync(properties);

        protected override Task HandleChallengeAsync(AuthenticationProperties properties) 
            => base.HandleChallengeAsync(properties);
    }
}
