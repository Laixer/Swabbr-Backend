using Microsoft.AspNetCore.Identity;
using Swabbr.Infrastructure.Data;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    // TODO Implement remaining methods
    public class UserStore : IUserStore<IdentityUserTableEntity>, IUserEmailStore<IdentityUserTableEntity>, IUserPhoneNumberStore<IdentityUserTableEntity>,
    IUserTwoFactorStore<IdentityUserTableEntity>, IUserPasswordStore<IdentityUserTableEntity>
    {
        private readonly IDbClientFactory _factory;

        public UserStore(IDbClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<IdentityUserTableEntity>("IdentityUser");
            await client.InsertEntityAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<IdentityUserTableEntity>("IdentityUser");
            await client.DeleteEntityAsync(user);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public async Task<IdentityUserTableEntity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            // TODO Implement
            return null;
        }

        public async Task<IdentityUserTableEntity> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<IdentityUserTableEntity>("IdentityUser");
            return await client.RetrieveEntityAsync(userId, userId);
        }

        public async Task<IdentityUserTableEntity> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await FindByEmailAsync(normalizedUserName, cancellationToken);
        }

        public Task<string> GetEmailAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public async Task<string> GetPasswordHashAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return user.PasswordHash.ToString();
        }

        public Task<string> GetPhoneNumberAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<bool> GetTwoFactorEnabledAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserId.ToString());
        }

        public Task<string> GetUserNameAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToString());
        }

        public Task<bool> HasPasswordAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(IdentityUserTableEntity user, string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(IdentityUserTableEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(IdentityUserTableEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(IdentityUserTableEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(IdentityUserTableEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(IdentityUserTableEntity user, string phoneNumber, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(IdentityUserTableEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(IdentityUserTableEntity user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(IdentityUserTableEntity user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUserTableEntity user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<IdentityUserTableEntity>("IdentityUser");
            await client.UpdateEntityAsync(user);
            return IdentityResult.Success;
        }
    }
}