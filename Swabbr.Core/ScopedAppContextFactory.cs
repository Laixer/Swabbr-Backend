using Swabbr.Core.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Core
{
    // TODO Unit tests
    /// <summary>
    ///     Scoped <see cref="IAppContextFactory"/> wrapper for which
    ///     the desired implementation can be specified for the rest
    ///     of the current scope.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Register all available <see cref="IAppContextFactory"/>
    ///         implementations as singletons for this to work as intended.
    ///         Other scenarios have not been validated or tested.
    ///     </para>
    ///     <para>
    ///         When no <see cref="SpecifiedImplementationType"/> is set during
    ///         runtime, <see cref="TDefaultAppContextFactory"/> is used.
    ///     </para>
    ///     <para>
    ///         TODO Fix this.
    ///         Be cautious when calling <see cref="SetAppContextFactoryType"/>.
    ///         There exists a possible race condition in the following situation:
    ///         <list type="bullet">
    ///             <item>A new scope is created.</item>
    ///             <item>Some services are resolved immediately.</item>
    ///             <item>Some of these services require an <see cref="AppContext"/>.</item>
    ///             <item>The <see cref="AppContext"/> is resolved immediately after scope creation.</item>
    ///             <item>Only after that <see cref="SetAppContextFactoryType"/> can be called.</item>
    ///         </list>
    ///         The current best fix is to call <see cref="SetAppContextFactoryType"/>
    ///         directly after scope creation before doing anything else.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TDefaultAppContextFactory">Default implementation to use.</typeparam>
    public class ScopedAppContextFactory<TDefaultAppContextFactory> : IScopedAppContextFactory
        where TDefaultAppContextFactory : class, IAppContextFactory
    {
        private readonly IEnumerable<IAppContextFactory> _implementations;

        /// <summary>
        ///     Indicates which implementation of <see cref="IAppContextFactory"/>
        ///     will be used when calling <see cref="CreateAppContext"/>.
        /// </summary>
        public Type SpecifiedImplementationType { get; private set; }

        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <param name="implementations"></param>
        public ScopedAppContextFactory(IEnumerable<IAppContextFactory> implementations) 
            => _implementations = implementations ?? throw new ArgumentNullException(nameof(implementations));

        /// <summary>
        ///     Creates an app context object using the implementation of
        ///     <see cref="IAppContextFactory"/> set in <see cref="SpecifiedImplementationType"/>.
        /// </summary>
        /// <remarks>
        ///     When no <see cref="SpecifiedImplementationType"/> is set during runtime, 
        ///     the first implementation available is used.
        /// </remarks>
        /// <returns>Result of <see cref="IAppContextFactory.CreateAppContext"/>.</returns>
        public AppContext CreateAppContext()
        {
            var implementation = (SpecifiedImplementationType != null)
                ? _implementations.First(x => x.GetType() == SpecifiedImplementationType)
                : _implementations.First(x => x.GetType() == typeof(TDefaultAppContextFactory));

            return implementation?.CreateAppContext() ?? throw new InvalidOperationException();
        }

        /// <summary>
        ///     Flag which implementation of <see cref="IAppContextFactory"/>
        ///     should be used for the rest of this scope for creation of
        ///     <see cref="AppContext"/> objects.
        /// </summary>
        /// <typeparam name="TAppContextFactory">Factory implementation.</typeparam>
        public void SetAppContextFactoryType<TAppContextFactory>()
            where TAppContextFactory : class, IAppContextFactory
        {
            SpecifiedImplementationType = typeof(TAppContextFactory);
        }
    }
}
