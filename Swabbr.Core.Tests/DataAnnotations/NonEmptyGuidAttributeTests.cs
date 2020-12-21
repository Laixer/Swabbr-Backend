using Swabbr.Core.DataAnnotations;
using System;
using Xunit;

namespace Swabbr.Core.Tests.DataAnnotations
{
    /// <summary>
    ///     Testcases for <see cref="NonEmptyGuidAttribute"/>.
    /// </summary>
    public class NonEmptyGuidAttributeTests : IClassFixture<NonEmptyGuidAttribute>
    {
        private readonly NonEmptyGuidAttribute _attribute;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NonEmptyGuidAttributeTests(NonEmptyGuidAttribute attribute)
            => _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

        [Fact]
        public void IsValidOnInput()
        { 
            // Assert.
            Assert.True(_attribute.IsValid(null));
            Assert.True(_attribute.IsValid(Guid.NewGuid()));
        }

        [Fact]
        public void IsInvalidOnInput() 
            // Assert.
            => Assert.False(_attribute.IsValid(Guid.Empty));

        [Theory]
        [InlineData(18548)]
        [InlineData('c')]
        [InlineData("hello")]
        public void IsInvalidOnNonGuid(object o)
            // Assert.
            => Assert.False(_attribute.IsValid(o));
    }
}
