using Swabbr.Core.Helpers;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Extensions;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     User refresh token repository.
    /// </summary>
    /// <remarks>
    ///     The refresh tokens are stored in the same way that Identity 
    ///     framework stores passwords, being a base64 encoded string of 
    ///     the (salt + hash) byte array. Because of this we need to get
    ///     the salt+hash string from the database in order to be able to
    ///     validate the hash. See <see cref="SecureHashHelper"/> for 
    ///     more explanation and references.
    /// </remarks>
    internal class UserRefreshTokenRepository : DatabaseContextBase, IUserRefreshTokenRepository
    {
        /// <summary>
        ///     Stores a refresh token hash for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="refreshToken">The unhashed refresh token.</param>
        public async Task StoreRefreshTokenHashAsync(Guid userId, string refreshToken)
        {
            var sql = @"
                UPDATE  application.user_up_to_date AS u
                SET     refresh_token_hash = @refresh_token_hash
                WHERE   u.id = @user_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);
            context.AddParameterWithValue("refresh_token_hash", SecureHashHelper.Hash(refreshToken));

            await context.NonQueryAsync(hasRowGuard: true);
        }

        // FUTURE: Validate this as an admin call using the appcontext.
        /// <summary>
        ///     Revokes a refresh token for a user if one is present.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public async Task RevokeRefreshTokenAsync(Guid userId)
        {
            var sql = @"
                UPDATE  application.user_up_to_date AS u
                SET     refresh_token_hash = null
                WHERE   u.id = @user_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await context.NonQueryAsync(hasRowGuard: true);
        }

        /// <summary>
        ///     Validates if a hashed refresh token matches the stored
        ///     refresh token hash in our data store.
        /// </summary>
        /// <remarks>
        ///     The hash value is retrieved from the database in order
        ///     to extract the salt used in the hashing algorithm.
        /// </remarks>
        /// <param name="userId">The user id.</param>
        /// <param name="refreshToken">The unhashed refresh token to check.</param>
        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var sql = @"
                SELECT  u.refresh_token_hash
                FROM    application.user_up_to_date AS u
                WHERE   u.id = @user_id";

            await using var context = await CreateNewDatabaseContext(sql);

            context.AddParameterWithValue("user_id", userId);

            await using var reader = await context.ReaderAsync();
            var saltWithHash = reader.GetSafeString(0);

            // Check hash.
            return SecureHashHelper.DoesHashMatch(saltWithHash, refreshToken);
        }
    }
}
