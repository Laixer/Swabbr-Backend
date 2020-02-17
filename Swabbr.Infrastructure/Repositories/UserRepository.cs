﻿using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUser"/> entities.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {

        private IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Gets a single <see cref="SwabbrUser"/> from the database based on its internal id.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="EntityNotFoundException"/> if the entity doesn't exist.
        /// </remarks>
        /// <param name="userId">Internal id</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableUser} WHERE id = '{userId}';";
                var result = await connection.QueryAsync<SwabbrUser>(sql);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with id = {userId}"); }
                else return result.First();
            }
        }

        /// <summary>
        /// Gets a single <see cref="SwabbrUser"/> based on its email.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="EntityNotFoundException"/> if the entity doesn't exist.
        /// </remarks>
        /// <param name="email">User email</param>
        /// <returns><see cref="SwabbrUser"/></returns>
        public async Task<SwabbrUser> GetByEmailAsync(string email)
        {
            email.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {TableUser} WHERE email = '{email}';";
                var result = await connection.QueryAsync<SwabbrUser>(sql);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with email = {email}"); }
                else return result.First();
            }
        }

        /// <summary>
        /// Retrieves our <see cref="UserSettings"/> object for a given <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        public async Task<UserSettings> GetUserSettingsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {ViewUserSettings} WHERE id = '{userId}';";
                var result = await connection.QueryAsync<UserSettings>(sql);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with id = {userId}"); }
                else return result.First();
            }
        }

        public Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }

}