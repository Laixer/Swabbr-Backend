using Swabbr.Core.DataAnnotations;
using System;
using Xunit;

namespace Swabbr.Core.Tests.DataAnnotations
{
    /// <summary>
    ///     Testcases for <see cref="Base64EncodedAttribute"/>.
    /// </summary>
    public class NonEmptyStringAttributeTests : IClassFixture<NonEmptyStringAttribute>
    {
        private readonly NonEmptyStringAttribute _attribute;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NonEmptyStringAttributeTests(NonEmptyStringAttribute attribute)
            => _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        [Theory]
        [InlineData(null)]
        [InlineData("hello")]
        public void IsValidOnInput(object o) 
            // Assert.
            => Assert.True(_attribute.IsValid(o));

        [Theory]
        [InlineData("")]
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
