using Swabbr.Core.Storage;
using System;
using Xunit;

namespace Swabbr.Core.Tests.Storage
{
    /// <summary>
    ///     Testscases for <see cref="StorageHelper"/>.
    /// </summary>
    /// <remarks>
    ///     This testcase exists to ensure consistency.
    /// </remarks>
    public class StorageHelperTests
    {
        [Fact]
        public void VideoFileNameIsCorrect()
        {
            // Arrange.
            var id = Guid.NewGuid();

            // Act.
            var fileName = StorageHelper.GetVideoFileName(id);

            // Assert.
            Assert.Equal(id.ToString(), fileName);
        }

        [Fact]
        public void ThumbnailFileNameIsCorrect()
        {
            // Arrange.
            var id = Guid.NewGuid();

            // Act.
            var fileName = StorageHelper.GetThumbnailFileName(id);

            // Assert.
            Assert.Equal($"{id}-thumbnail", fileName);
        }
    }
}
