using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.Core.Configuration;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{
    public class AMSReactionWithThumbnailService : AMSReactionService, IReactionWithThumbnailService
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AMSReactionWithThumbnailService(IReactionRepository reactionRepository,
            IVlogRepository vlogRepository,
            IUserRepository userRepository,
            IStorageService storageService,
            INotificationService notificationService,
            AMSClient amsClient,
            IOptions<SwabbrConfiguration> optionsSwabbr)
            : base(reactionRepository, vlogRepository, userRepository, storageService, notificationService, amsClient, optionsSwabbr)
        {
        }

        /// <summary>
        ///     Gets a <see cref="Entities.Reaction"/> with its corresponding 
        ///     thumbnail details.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Entities.Reaction"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        public async Task<ReactionWithThumbnailDetails> GetWithThumbnailDetailsAsync(Guid reactionId)
            => new ReactionWithThumbnailDetails
            {
                Reaction = await GetReactionAsync(reactionId).ConfigureAwait(false),
                ThumbnailUri = await _storageService.GetDownloadAccessUriForReactionThumbnailAsync(reactionId).ConfigureAwait(false)
            };

        /// <summary>
        ///     Gets all <see cref="Entities.Reaction"/> with its corresponding 
        ///     thumbnail details for a given <paramref name="vlogId"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Entities.Vlog"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        public async Task<IEnumerable<ReactionWithThumbnailDetails>> GetWithThumbnailForVlogAsync(Guid vlogId)
        {
            var reactions = await GetReactionsForVlogAsync(vlogId).ConfigureAwait(false);
            var result = new ConcurrentBag<ReactionWithThumbnailDetails>();

            Parallel.ForEach(reactions, (reaction) =>
            {
                result.Add(new ReactionWithThumbnailDetails
                {
                    Reaction = reaction,
                    ThumbnailUri = Task.Run(() => _storageService.GetDownloadAccessUriForReactionThumbnailAsync(reaction.Id)).Result
                });
            });

            return result;
        }
    }
}
