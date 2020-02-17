using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos.Table;
using Swabbr.Infrastructure.Data;
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

        public async Task<IdentityResult> CreateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
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

        public async Task<IdentityResult> UpdateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //! Important: Merging the entity because we do not want to discard the existing properties
            //await client.MergeEntityAsync(user);
            // return IdentityResult.Success;
            throw new NotImplementedException(); ;
        }

        public async Task<IdentityResult> DeleteAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //await client.DeleteEntityAsync(user);
            //return IdentityResult.Success;
            throw new NotImplementedException();
        }

        public async Task<SwabbrIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            //var client = _factory.GetClient<SwabbrIdentityUser>(UsersTableName);
            //return await client.RetrieveEntityAsync(userId, userId);
            throw new NotImplementedException();
        }

        public async Task<SwabbrIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            // Usernames are e-mails in this application.
            //return await FindByEmailAsync(normalizedUserName, cancellationToken);
            throw new NotImplementedException();
        }

        public async Task<string> GetNormalizedUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.NormalizedEmail;
        }

        public async Task<string> GetPasswordHashAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        public async Task<string> GetPhoneNumberAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumber;
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.PhoneNumberConfirmed;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.TwoFactorEnabled;
        }

        public async Task<string> GetUserIdAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        public async Task<bool> HasPasswordAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return !string.IsNullOrEmpty(user.PasswordHash);
        }

        public Task SetNormalizedUserNameAsync(SwabbrIdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task SetPasswordHashAsync(SwabbrIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
        }

        public async Task SetPhoneNumberAsync(SwabbrIdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
        }

        public async Task SetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
        }

        public async Task SetTwoFactorEnabledAsync(SwabbrIdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
        }

        public Task SetUserNameAsync(SwabbrIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            // Username == Email
            return Task.CompletedTask;
        }

        #region IUserEmailStore

        public async Task<SwabbrIdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
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

        public async Task<string> GetEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.EmailConfirmed;
        }

        public async Task<string> GetNormalizedEmailAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return user.NormalizedEmail;
        }

        public async Task SetEmailAsync(SwabbrIdentityUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
        }

        public async Task SetEmailConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
        }

        public async Task SetNormalizedEmailAsync(SwabbrIdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
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

        public async Task<IList<Claim>> GetClaimsAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            return new List<Claim>();
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
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //!IMPORTANT
            //TODO: Hardcoded. Roles are not being stored yet. For testing purposes only.
            return new List<string>
            {
                "User"
            };
            throw new NotImplementedException();
        }

        public async Task<IList<SwabbrIdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return new List<SwabbrIdentityUser>();
            throw new NotImplementedException();
        }

        public async Task<bool> IsInRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return (roleName == "user");
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO: Not implemented
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        #endregion IUserRoleStore
    }
}