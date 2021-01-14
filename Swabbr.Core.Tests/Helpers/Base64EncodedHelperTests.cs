using Swabbr.Core.Helpers;
using Xunit;

namespace Swabbr.Core.Tests.Helpers
{
    /// <summary>
    ///     Testing functionality for base 64 encoded strings.
    /// </summary>
    public class Base64EncodedHelperTests
    {
        [Theory]
        [InlineData("c2RrbGpmaGRza2xmamg=")]
        [InlineData("ZmxvcnMgaGVlZnQgZWVuIDggdm9vciB6biBhZnN0dWRy")]
        [InlineData("eA==")]
        public void Base64EncodedIsValid(string s)
            // Assert.
            => Assert.True(Base64EncodedHelper.IsBase64Encoded(s));

        [Theory]
        [InlineData("")]
        [InlineData("c2RrbGpmaGRza2xmamg")]
        [InlineData("123")]
        public void NotBase64EncodedIsInvalid(string s) 
            // Assert.
            => Assert.False(Base64EncodedHelper.IsBase64Encoded(s));
    }
}
