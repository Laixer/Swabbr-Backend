using Laixer.Infra.Npgsql;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public sealed class LivestreamRepository : ILivestreamRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LivestreamRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        public Task<Livestream> GetActiveLivestreamForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Livestream>> GetActiveLivestreamsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Livestream> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Livestream> GetByIdAsync(string livestreamId)
        {
            throw new NotImplementedException();
        }

        public Task<Livestream> ReserveLivestreamForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
