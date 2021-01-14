namespace Swabbr.Core.Abstractions
{
    /// <summary>
    ///     Base class for a service that
    ///     uses our application context.
    /// </summary>
    public abstract class AppServiceBase
    {
        /// <summary>
        ///     Application context.
        /// </summary>
        public AppContext AppContext { get; set; }
    }
}
