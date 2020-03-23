using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="Request"/> entities.
    /// </summary>
    public sealed class RequestRepository : IRequestRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public RequestRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Creates a <see cref="Request"/> in our database.
        /// </summary>
        /// <param name="entity"><see cref="Request"/></param>
        /// <returns>Created and queried <see cref="Request"/></returns>
        public async Task<Request> CreateAsync(Request entity)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            entity.Id.ThrowIfNotNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $@"
                    INSERT INTO {TableRequest} (
                        user_id
                    ) VALUES (
                        @UserId
                    ) RETURNING id";
                var id = await connection.ExecuteScalarAsync<Guid>(sql, entity).ConfigureAwait(false);
                id.ThrowIfNullOrEmpty();

                // TODO We can't call GetAsync(id) here when we're in a transaction scope.
                entity.Id = id;
                return entity;
            }
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a single <see cref="RequestRecordVlog"/> from our database.
        /// </summary>
        /// <param name="id">Internal <see cref="Request"/> id</param>
        /// <returns><see cref="RequestRecordVlog"/></returns>
        public Task<RequestRecordVlog> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            return SharedRepositoryFunctions.GetAsync<RequestRecordVlog>(_databaseProvider, id, TableRequest);
        }

        /// <summary>
        /// Updates the <see cref="RequestState"/> for a <see cref="Request"/>.
        /// </summary>
        /// <param name="requestId">Internal <see cref="Request"/> id</param>
        /// <param name="state"><see cref="RequestState"/></param>
        /// <returns></returns>
        public async Task<Request> MarkAsync(Guid requestId, RequestState state)
        {
            requestId.ThrowIfNullOrEmpty();

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                // TODO SQL Injection
                var sql = $@"UPDATE {TableRequest} SET request_state = '{state.GetEnumMemberAttribute()}' WHERE id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = requestId }).ConfigureAwait(false);
                if (rowsAffected == 0) { throw new EntityNotFoundException(); }
                if (rowsAffected > 1) { throw new InvalidOperationException("Found multiple entities on single get"); }
                return await GetAsync(requestId).ConfigureAwait(false);
            }
        }

        public Task<Request> UpdateAsync(Request entity)
        {
            throw new NotImplementedException();
        }
    }

}
