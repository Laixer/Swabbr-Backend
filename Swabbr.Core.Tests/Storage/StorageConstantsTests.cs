using Swabbr.Core.Storage;
using Xunit;

namespace Swabbr.Core.Tests.Storage
{
    /// <summary>
    ///     Testscases for <see cref="StorageConstants"/>.
    /// </summary>
    /// <remarks>
    ///     These testcases exist to ensure consistency.
    /// </remarks>
    public class StorageConstantsTests
    {
        [Fact]
        public void ConstantsNotModified()
        {
            // Assert.
            Assert.Equal("reactions", StorageConstants.ReactionStorageFolderName);
            Assert.Equal("vlogs", StorageConstants.VlogStorageFolderName);
        }
    }
}
