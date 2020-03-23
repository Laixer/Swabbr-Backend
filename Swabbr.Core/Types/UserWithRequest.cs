using Swabbr.Core.Entities;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Wrapper class for a <see cref="SwabbrUser"/> and a corresponding
    /// <see cref="Request"/>.
    /// </summary>
    public sealed class UserWithRequest
    {

        public SwabbrUser User { get; set; }

        public Request Request { get; set; }

    }

}
