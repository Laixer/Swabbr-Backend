using Microsoft.AspNetCore.Authentication;
using Swabbr.Core.Entities;
using Swabbr.Testing.Faker.Entities;

namespace Swabbr.IntegrationTests.Authentication
{
    /// <summary>
    ///     Authentication scheme options for testing scenarios.
    /// </summary>
    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        ///     Authenticated user.
        /// </summary>
        public User User { get; set; } = new UserFaker().Generate();
    }
}
