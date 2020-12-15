using Microsoft.AspNetCore.Identity;
using Swabbr.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    // TODO Rework or remove completely
    /// <summary>
    ///     Used by identity framework to store our users.
    /// </summary>
    public class UserStore :
        IUserStore<SwabbrIdentityUser>,
        IUserEmailStore<SwabbrIdentityUser>,
        IUserPhoneNumberStore<SwabbrIdentityUser>,
        IUserTwoFactorStore<SwabbrIdentityUser>,
        IUserPasswordStore<SwabbrIdentityUser>,
        IUserClaimStore<SwabbrIdentityUser>,
        IUserRoleStore<SwabbrIdentityUser>
    {
        public static string UsersTableName => "Users";
        public static string UserSettingsTableName => "UserSettings";

        public Task<IdentityResult> CreateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<IdentityResult> UpdateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //! Important: Merging the entity because we do not want to discard the existing properties
            //await client.MergeEntityAsync(user);
            // return IdentityResult.Success;
            throw new NotImplementedException(); ;
        }

        public Task<IdentityResult> DeleteAsync(SwabbrIdentityUser user, CancellationToken cancellationToken) =>
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //await client.DeleteEntityAsync(user);
            //return IdentityResult.Success;
            throw new NotImplementedException();

        public Task<SwabbrIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //return await client.RetrieveEntityAsync(userId, userId);
            throw new NotImplementedException();

        public Task<SwabbrIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            // Usernames are e-mails in this application.
            //return await FindByEmailAsync(normalizedUserName, cancellationToken);
            throw new NotImplementedException();

        public Task<string> GetNormalizedUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetPasswordHashAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<bool> GetTwoFactorEnabledAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetNormalizedUserNameAsync(SwabbrIdentityUser user, string normalizedName, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task SetPasswordHashAsync(SwabbrIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(SwabbrIdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(SwabbrIdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(SwabbrIdentityUser user, string userName, CancellationToken cancellationToken) =>
            // Username == Email
            // TODO Is this correct?
            Task.CompletedTask;

        #region IUserEmailStore

        public Task<SwabbrIdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
            /*var table = _factory.GetClient<SwabbrIdentityUser>(UsersTableName).TableReference;

// Check if the normalized email already exists in the table.
var tq = new TableQuery<SwabbrIdentityUser>().Where(
TableQuery.GenerateFilterCondition("NormalizedEmail", QueryComparisons.Equal, normalizedEmail));

var queryResults = table.ExecuteQuery(tq);

if (queryResults.Any())
{
return queryResults.First();
}

return null;*/
            throw new NotImplementedException();

        public Task<string> GetEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(SwabbrIdentityUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(SwabbrIdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        #endregion IUserEmailStore

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //TODO: dispose managed state (managed objects).
                }

                //TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                //TODO: set large fields to null.

                disposedValue = true;
            }
        }

        //TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserStore() { // Do not change this code. Put cleanup code in Dispose(bool disposing)
        // above. Dispose(false); }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            //TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        #region IUserClaimStore

        public Task AddClaimsAsync(SwabbrIdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (claims == null) { throw new ArgumentNullException(nameof(claims)); }
            //foreach (Claim c in claims)
            //{
                //TODO: Store claim for user
            //}
            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(SwabbrIdentityUser user, CancellationToken cancellationToken) => Task.FromResult<IList<Claim>>(new List<Claim>());

        public Task<IList<SwabbrIdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task RemoveClaimsAsync(SwabbrIdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (claims == null) { throw new ArgumentNullException(nameof(claims)); }
            //foreach (Claim c in claims)
            //{
                //TODO: Remove claim for user
            //}
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(SwabbrIdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) =>
            //TODO: Update claim
            throw new NotImplementedException();

        #endregion IUserClaimStore

        #region IUserRoleStore

        public Task AddToRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken) =>
            //TODO:Not implemented
            Task.CompletedTask;

        public Task<IList<string>> GetRolesAsync(SwabbrIdentityUser user, CancellationToken cancellationToken) =>
            //!IMPORTANT
            //TODO: Hardcoded. Roles are not being stored yet. For testing purposes only.
            Task.FromResult<IList<string>>(new List<string>
            {
                "User"
            });

        public Task<IList<SwabbrIdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) =>
            //TODO: Not implemented
            Task.FromResult<IList<SwabbrIdentityUser>>(new List<SwabbrIdentityUser>());

        public Task<bool> IsInRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken) =>
            //TODO: Not implemented
            Task.FromResult(roleName == "user");

        public Task RemoveFromRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken) =>
            //TODO: Not implemented
            Task.CompletedTask;

        #endregion IUserRoleStore
    }
}
