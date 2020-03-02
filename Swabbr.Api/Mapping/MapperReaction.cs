using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using System;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="Reaction"/> entities.
    /// </summary>
    internal static class MapperReaction
    {

        internal static ReactionOutputModel Map(Reaction reaction)
        {
            if (reaction == null) { throw new ArgumentNullException(nameof(reaction)); }
            return new ReactionOutputModel
            {
                CreateDate = reaction.CreateDate,
                Id = reaction.Id,
                IsPrivate = reaction.IsPrivate,
                UserId = reaction.UserId,
                TargetVlogId = reaction.TargetVlogId
            };
        }

    }

}
