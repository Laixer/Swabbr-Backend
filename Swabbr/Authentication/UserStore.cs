using Microsoft.AspNetCore.Identity;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    public class UserStore : IUserStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>,
    IUserTwoFactorStore<User>, IUserPasswordStore<User>
    {
        private readonly IUserRepository _repository;

        public UserStore(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(user);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Nothing to dispose of.
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _repository.GetByEmailAsync(normalizedEmail);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(new Guid(userId));
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _repository.GetByUsernameAsync(normalizedUserName);
        }

        // TODO: ???
        public async Task<string> GetEmailAsync(User user, CancellationToken cancellationToken) => user.Email;

        public async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken) => user.EmailConfirmed;

        public async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken) => user.Email.ToLower();

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => user.Username.ToLower();

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) => user.PasswordHash;

        public async Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken) => user.PhoneNumber;

        public async Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken) => user.PhoneNumberVerified;

        public async Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken) => user.TwoFactorEnabled;

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) => user.UserId.ToString();

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) => user.Email;

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return true;
        }

        public async Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
        }

        public async Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            // TODO
            //throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(user);
            return IdentityResult.Success;
        }
    }
}
