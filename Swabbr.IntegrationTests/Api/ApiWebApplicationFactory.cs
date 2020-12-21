namespace Swabbr.IntegrationTests.Api
{
    /// <summary>
    ///     Creates an API web application with authentication.
    /// </summary>
    public class ApiAuthWebApplicationFactory : AuthWebApplicationFactory<Startup>
    {
    }

    /// <summary>
    ///     Creates an API web application without authentication.
    /// </summary>
    public class ApiWebApplicationFactory : CustomWebApplicationFactory<Startup>
    {
    }
}
