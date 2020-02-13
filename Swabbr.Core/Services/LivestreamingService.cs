using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public class LivestreamingService : ILivestreamingService
    {
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogRepository _vlogRepository;

        private readonly ILivestreamingClient _livestreamClient;

        public LivestreamingService(
            ILivestreamRepository livestreamRepository,
            IVlogRepository vlogRepository,
            ILivestreamingClient livestreamClient
            )
        {
            _livestreamRepository = livestreamRepository;
            _vlogRepository = vlogRepository;
            _livestreamClient = livestreamClient;
        }

        public async Task<Livestream> CreateLivestreamAsync(string name)
        {
            var newStream = await _livestreamClient.CreateNewStreamAsync(name);

            // Store the new stream in the livestream repository
            // TODO THOMAS This is dangerous! We can now create a livestream without storing it in our database.
            // It might be a good idea to let the live stream client handle this functionality, and prevent the
            // very plausible case of losing livestreams because we "forget" to store after creation.
            var createdStream = await _livestreamRepository.CreateAsync(newStream);

            return createdStream;
        }

        public async Task DeleteStreamAsync(string id)
        {
            await _livestreamClient.DeleteLivestreamAsync(id);
        }

        // TODO THOMAS Transactional, this entire function is a giant race condition.
        public async Task<LivestreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
            int availableStreamCount = 1;

            // If no streams are available, create a new stream. 
            //TODO: Minimum amount/lower boundary needed instead of 0?
            if (availableStreamCount < 1)
            {
                await CreateLivestreamAsync("test");
            }

            // Check if there are available (unreserved) livestreams in storage.
            var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);

            // Retrieve live stream connection details from the api.
            var connection = await GetStreamConnectionAsync(availableLivestream.ExternalId);
            return connection;
        }

        public Task<LivestreamConnectionDetails> GetStreamConnectionAsync(string id)
        {
            return _livestreamClient.GetStreamConnectionAsync(id);
        }

        public Task<LivestreamPlaybackDetails> GetStreamPlaybackAsync(string id)
        {
            return _livestreamClient.GetStreamPlaybackAsync(id);
        }

        public Task StartStreamAsync(string id)
        {
            return _livestreamClient.StartLivestreamAsync(id);
        }

        public async Task StopStreamAsync(string id)
        {
            await _livestreamClient.StopLivestreamAsync(id);
        }

        public async Task<string> GetThumbnailUrlAsync(string id)
        {
            return await _livestreamClient.GetThumbnailUrlAsync(id);
        }

        public Task SyncRecordingsForVlogAsync(string livestreamId, Guid vlogId)
        {
            throw new NotImplementedException();
        }
    }
}
