using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    // TODO This should be cleaned up completely / removed completely
    /// <summary>
    ///     Contains our role stores for identity.
    /// </summary>
    public class RoleStore : IRoleStore<SwabbrIdentityRole>
    {
        protected static readonly List<SwabbrIdentityRole> Roles = new List<SwabbrIdentityRole>
        {
            new SwabbrIdentityRole{ Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER" },
            new SwabbrIdentityRole{ Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
        };

        public Task<IdentityResult> CreateAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<SwabbrIdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Roles.Where(r => r.Id.ToString() == roleId).First());
        }

        public Task<SwabbrIdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(Roles.Where(r => r.NormalizedName.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase)).First());
        }

        public Task<string> GetNormalizedRoleNameAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            if (role == null) { throw new ArgumentNullException(nameof(role)); }
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            if (role == null) { throw new ArgumentNullException(nameof(role)); }
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            if (role == null) { throw new ArgumentNullException(nameof(role)); }
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(SwabbrIdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null) { throw new ArgumentNullException(nameof(role)); }
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(SwabbrIdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null) { throw new ArgumentNullException(nameof(role)); }
            role.Name = roleName;
            return Task.CompletedTask;
        }

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
        // ~RoleStore() { // Do not change this code. Put cleanup code in Dispose(bool disposing)
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
    }
}
