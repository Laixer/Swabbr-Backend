using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{

    // TODO: Not optimized
    // TODO THOMAS This is a lot of code, might be worth revisiting.
    // TODO THOMAS Rename to SwabbrUserStore oid! (yorick)
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

        public Task<IdentityResult> CreateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            /*var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);

            // Ensure user does not exist
            // TODO THOMAS This should be success or throw
            var checkUser = await FindByEmailAsync(user.NormalizedEmail, new CancellationToken());
            if (checkUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists." });
            }

            // Insert the user
            // TODO THOMAS This should be success or throw
            var insertedUser = await client.InsertEntityAsync(user);

            if (insertedUser != null)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "Could not insert user" });*/
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //! Important: Merging the entity because we do not want to discard the existing properties
            //await client.MergeEntityAsync(user);
            // return IdentityResult.Success;
            throw new NotImplementedException(); ;
        }

        public Task<IdentityResult> DeleteAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //await client.DeleteEntityAsync(user);
            //return IdentityResult.Success;
            throw new NotImplementedException();
        }

        public Task<SwabbrIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //return await client.RetrieveEntityAsync(userId, userId);
            throw new NotImplementedException();
        }

        public Task<SwabbrIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            // Usernames are e-mails in this application.
            //return await FindByEmailAsync(normalizedUserName, cancellationToken);
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetPasswordHashAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<bool> GetTwoFactorEnabledAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetNormalizedUserNameAsync(SwabbrIdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            // TODO Is this correct?
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(SwabbrIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(SwabbrIdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(SwabbrIdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(SwabbrIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            // Username == Email
            // TODO Is this correct?
            return Task.CompletedTask;
        }

        #region IUserEmailStore

        public Task<SwabbrIdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
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
        }

        public Task<string> GetEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(SwabbrIdentityUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(SwabbrIdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
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
            foreach (Claim c in claims)
            {
                //TODO: Store claim for user
            }
            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public Task<IList<SwabbrIdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            //TODO: ?
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(SwabbrIdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (Claim c in claims)
            {
                //TODO: Remove claim for user
            }
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(SwabbrIdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            //TODO: Update claim
            throw new NotImplementedException();
        }

        #endregion IUserClaimStore

        #region IUserRoleStore

        public Task AddToRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO:Not implemented
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //!IMPORTANT
            //TODO: Hardcoded. Roles are not being stored yet. For testing purposes only.
            return Task.FromResult<IList<string>>(new List<string>
            {
                "User"
            });
        }

        public async Task<IList<SwabbrIdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return new List<SwabbrIdentityUser>();
        }

        public Task<bool> IsInRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return Task.FromResult(roleName == "user");
        }

        public Task RemoveFromRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return Task.CompletedTask;
        }

        #endregion IUserRoleStore
    }
}