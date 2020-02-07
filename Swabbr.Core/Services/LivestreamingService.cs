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

        public async Task<Livestream> CreateNewStreamAsync(string name)
        {
            var newStream = await _livestreamClient.CreateNewStreamAsync(name);

            // Store the new stream in the livestream repository
            var createdStream = await _livestreamRepository.CreateAsync(newStream);

            return createdStream;
        }

        public async Task DeleteStreamAsync(string id)
        {
            await _livestreamClient.DeleteStreamAsync(id);
        }

        public async Task<StreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId)
        {
            int availableStreamCount = await _livestreamRepository.GetAvailableLivestreamCountAsync();

            // If no streams are available, create a new stream. 
            //TODO: Minimum amount/lower boundary needed instead of 0?
            if (availableStreamCount < 1)
            {
                await this.CreateNewStreamAsync("test");
            }

            // Check if there are available (unreserved) livestreams in storage.
            var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);

            // Retrieve live stream connection details from the api.
            var connection = await GetStreamConnectionAsync(availableLivestream.Id);
            return connection;
        }

        public Task<StreamConnectionDetails> GetStreamConnectionAsync(string id)
        {
            return _livestreamClient.GetStreamConnectionAsync(id);
        }

        public Task<StreamPlaybackDetails> GetStreamPlaybackAsync(string id)
        {
            return _livestreamClient.GetStreamPlaybackAsync(id);
        }

        public Task StartStreamAsync(string id)
        {
            return _livestreamClient.StartStreamAsync(id);
        }

        public async Task StopStreamAsync(string id)
        {
            await _livestreamClient.StopStreamAsync(id);
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
