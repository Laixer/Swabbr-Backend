using Swabbr.Core.DataAnnotations;
using System;
using Xunit;

namespace Swabbr.Core.Tests.DataAnnotations
{
    /// <summary>
    ///     Testcases for <see cref="Base64EncodedAttribute"/>.
    /// </summary>
    public class Base64EncodedAttributeTests : IClassFixture<Base64EncodedAttribute>
    {
        private readonly Base64EncodedAttribute _attribute;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public Base64EncodedAttributeTests(Base64EncodedAttribute attribute)
            => _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        [Theory]
        [InlineData(null)]
#if DEBUG
        [InlineData("c2pmaGRza2xmamg=")]
        [InlineData("ZmxvcnMgaGVlZnQgZWVuIDB6biBhZnN0dWRy")]
        [InlineData("eA==")]
#endif
        public void IsValidOnInput(object o) 
            // Assert.
            => Assert.True(_attribute.IsValid(o));

        [Theory]
        [InlineData("")]
#if DEBUG
        [InlineData("c2RrbGpmhhhhaGRza2xmamg")]
        [InlineData("123948")]
#endif
        public void IsInvalidOnInput(object o) 
            // Assert.
            => Assert.False(_attribute.IsValid(o));

        [Theory]
        [InlineData(53)]
        [InlineData('c')]
        public void IsInvalidOnNonString(object o)
            // Assert.
            => Assert.False(_attribute.IsValid(o));
    }
}
