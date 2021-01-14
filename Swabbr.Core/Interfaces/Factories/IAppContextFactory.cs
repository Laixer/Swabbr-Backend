namespace Swabbr.Core.Interfaces.Factories
{
    /// <summary>
    ///     Contract for an app context factory.
    /// </summary>
    public interface IAppContextFactory
    {
        /// <summary>
        ///     Creates a new instance of an application context.
        /// </summary>
        /// <remarks>
        ///     Do not directly inject this factory into any classes.
        ///     Use <see cref="IScopedAppContextFactory"/> instead to 
        ///     prevent uncontrolled app context creation in a scope.
        ///     The mentioned class should be used to configure which
        ///     <see cref="IAppContextFactory"/> implemenation should 
        ///     be used during a scope.
        /// </remarks>
        /// <returns>App context object.</returns>
        Core.AppContext CreateAppContext();
    }
}
