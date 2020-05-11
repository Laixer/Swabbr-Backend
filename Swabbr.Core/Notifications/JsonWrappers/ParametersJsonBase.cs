namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Abstract base class for creating operation specific JSON wrappers.
    /// </summary>
    public abstract class ParametersJsonBase
    {

        public string Title { get; set; }

        public string Message { get; set; }

    }

}
