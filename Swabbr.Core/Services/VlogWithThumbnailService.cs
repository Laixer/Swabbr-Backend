using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public sealed class VlogWithThumbnailService : VlogService, IVlogWithThumbnailService
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogWithThumbnailService(IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IStorageService storageService)
            : base(vlogRepository, vlogLikeRepository, userRepository, notificationService, storageService)
        {
        }

        /// <summary>
        ///     Gets all recommended <see cref="Entities.Vlog"/>s for a given <see cref="Entities.SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="Entities.SwabbrUser"/> id</param>
        /// <param name="maxCount">Max result set size</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/> collection</returns>
        public async Task<IEnumerable<VlogWithThumbnailDetails>> GetRecommendedVlogsWithThumbnailDetailsForUserAsync(Guid userId, uint maxCount)
        {
            var vlogs = await GetRecommendedForUserAsync(userId, maxCount).ConfigureAwait(false);
            var result = new ConcurrentBag<VlogWithThumbnailDetails>();

            Parallel.ForEach(vlogs, (vlog) =>
            {
                result.Add(new VlogWithThumbnailDetails
                {
                    Vlog = vlog,
                    ThumbnailUri = Task.Run(() => _storageService.GetDownloadAccessUriForVlogThumbnailAsync(vlog.Id)).Result
                });
            });

            return result;
        }

        /// <summary>
        ///     Gets all <see cref="Entities.Vlog"/>s for a given <see cref="Entities.SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/> collection</returns>
        public async Task<IEnumerable<VlogWithThumbnailDetails>> GetVlogsWithThumbnailDetailsFromUserAsync(Guid userId)
        {
            var vlogs = await GetVlogsFromUserAsync(userId).ConfigureAwait(false);
            var result = new ConcurrentBag<VlogWithThumbnailDetails>();

            Parallel.ForEach(vlogs, (vlog) =>
            {
                result.Add(new VlogWithThumbnailDetails
                {
                    Vlog = vlog,
                    ThumbnailUri = Task.Run(() => _storageService.GetDownloadAccessUriForVlogThumbnailAsync(vlog.Id)).Result
                });
            });

            return result;
        }

        /// <summary>
        ///     Gets a <see cref="Entities.Vlog"/> with its corresponding 
        ///     thumbnail details.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Entities.Vlog"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        public async Task<VlogWithThumbnailDetails> GetWithThumbnailDetailsAsync(Guid vlogId)
            => new VlogWithThumbnailDetails
            {
                Vlog = await GetAsync(vlogId).ConfigureAwait(false),
                ThumbnailUri = await _storageService.GetDownloadAccessUriForVlogThumbnailAsync(vlogId).ConfigureAwait(false)
            };
    }
}
