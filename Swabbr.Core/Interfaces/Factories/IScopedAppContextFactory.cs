namespace Swabbr.Core.Interfaces.Factories
{
    /// <summary>
    ///     Scoped <see cref="IAppContextFactory"/> wrapper for which
    ///     the desired implementation can be specified for the rest
    ///     of the current scope.
    /// </summary>
    /// <remarks>
    ///     This abstraction exists to enable the usage of generics 
    ///     for specifying the default <see cref="IAppContextFactory"/> 
    ///     impmlementation.
    /// </remarks>
    public interface IScopedAppContextFactory
    {
        /// <summary>
        ///     Creates a new instance of an 
        ///     application context.
        /// </summary>
        /// <param name="parameters">Additional creation parameters.</param>
        /// <returns>Application context.</returns>
        AppContext CreateAppContext();

        /// <summary>
        ///     Flag which implementation of <see cref="IAppContextFactory"/>
        ///     should be used for the rest of this scope for creation of
        ///     <see cref="AppContext"/> objects.
        /// </summary>
        /// <typeparam name="TAppContextFactory">Factory implementation.</typeparam>
        void SetAppContextFactoryType<TAppContextFactory>()
            where TAppContextFactory : class, IAppContextFactory;
    }
}
