using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a repository managing refresh tokens.
    /// </summary>
    public interface IUserRefreshTokenRepository
    {
        /// <summary>
        ///     Stores a refresh token hash for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="refreshToken">The unhashed refresh token.</param>
        Task StoreRefreshTokenHashAsync(Guid userId, string refreshToken);

        /// <summary>
        ///     Revokes a refresh token for a user if one is present.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task RevokeRefreshTokenAsync(Guid userId);

        /// <summary>
        ///     Validates if a hashed refresh token matches the stored
        ///     refresh token hash in our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="refreshToken">The token to check, unhashed.</param>
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    }
}
