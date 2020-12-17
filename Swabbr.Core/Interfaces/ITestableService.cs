using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    ///     Contract for making a service testable.
    /// </summary>
    public interface ITestableService
    {
        /// <summary>
        ///     Validates that some service is 
        ///     working as it should.
        /// </summary>
        public Task TestServiceAsync();
    }
}
