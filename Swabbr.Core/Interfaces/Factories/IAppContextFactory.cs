namespace Swabbr.Core.Interfaces.Factories
{
    /// <summary>
    ///     Contract for an app context factory.
    /// </summary>
    public interface IAppContextFactory
    {
        /// <summary>
        ///     Creates a new instance of an 
        ///     application context.
        /// </summary>
        /// <returns>Application context.</returns>
        AppContext CreateAppContext();
    }
}
