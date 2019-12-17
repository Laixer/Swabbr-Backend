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
    public class UserStore : 
        IUserStore<SwabbrIdentityUser>, 
        IUserEmailStore<SwabbrIdentityUser>, 
        IUserPhoneNumberStore<SwabbrIdentityUser>,
        IUserTwoFactorStore<SwabbrIdentityUser>, 
        IUserPasswordStore<SwabbrIdentityUser>, 
        IUserClaimStore<SwabbrIdentityUser>,
        IUserRoleStore<SwabbrIdentityUser>
    {
        private readonly IDbClientFactory _factory;

        public UserStore(IDbClientFactory factory)
        {
            _factory = factory;
        }

        public static string UserTableName => "Users";

        public async Task<IdentityResult> CreateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<SwabbrIdentityUser>(UserTableName);
            await client.InsertOrMergeEntityAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<SwabbrIdentityUser>(UserTableName);
            //! Important: Merging the entity because we do not want to discard the existing properties
            await client.MergeEntityAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<SwabbrIdentityUser>(UserTableName);
            await client.DeleteEntityAsync(user);
            return IdentityResult.Success;
        }

        public async Task<SwabbrIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var client = _factory.GetClient<SwabbrIdentityUser>(UserTableName);
            return await client.RetrieveEntityAsync(userId, userId);
        }

        public async Task<SwabbrIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            // Usernames are e-mails in this application.
            return await FindByEmailAsync(normalizedUserName, cancellationToken);
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
            return user.UserId.ToString();
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
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(SwabbrIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(SwabbrIdentityUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(SwabbrIdentityUser user, bool enabled, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(SwabbrIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        #region IUserEmailStore
        public async Task<SwabbrIdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var table = _factory.GetClient<SwabbrIdentityUser>(UserTableName).CloudTableReference;

            var tq = new TableQuery<SwabbrIdentityUser>().Where(
                TableQuery.GenerateFilterCondition("NormalizedEmail", QueryComparisons.Equal, normalizedEmail));

            var queryResults = table.ExecuteQuery(tq);

            if (queryResults.Any())
            {
                return queryResults.First();
            }

            return null;
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

        public Task SetEmailAsync(SwabbrIdentityUser user, string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(SwabbrIdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(SwabbrIdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);//TODO Implement Method
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserStore()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IUserClaimStore
        public Task AddClaimsAsync(SwabbrIdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach(Claim c in claims)
            {
                // TODO insert c for user
            }
            return Task.CompletedTask;
        }

        public async Task<IList<Claim>> GetClaimsAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            // TODO?
            return new List<Claim>();
        }

        public Task<IList<SwabbrIdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            // TODO?
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(SwabbrIdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (Claim c in claims)
            {
                // TODO remove c for user
            }
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(SwabbrIdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            // TODO Update c
            throw new NotImplementedException();
        }
        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            // TODO
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(SwabbrIdentityUser user, CancellationToken cancellationToken)
        {
            //TODO
            return new List<string>();
            throw new NotImplementedException();
        }

        public async Task<IList<SwabbrIdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            //TODO
            return new List<SwabbrIdentityUser>();
            throw new NotImplementedException();
        }

        public async Task<bool> IsInRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            //TODO
            return false;
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(SwabbrIdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }
        #endregion
    }
}