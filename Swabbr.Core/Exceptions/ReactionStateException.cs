using Laixer.Utility.Extensions;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Exceptions
{

    /// <summary>
    /// Indicates the <see cref="Entities.Reaction"/> status is invalid for
    /// a given operation.
    /// </summary>
    public sealed class ReactionStateException : Exception
    {

        public ReactionStateException() { }

        public ReactionStateException(string message) : base(message) { }

        public ReactionStateException(string message, Exception innerException) : base(message, innerException) { }

        public ReactionStateException(ReactionState notInThisState)
            : base($"Reaction not in {notInThisState.GetEnumMemberAttribute()} state") { }

    }

}
