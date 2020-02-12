using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    //TODO: Hardcoded for now, not yet able to create, update or delete roles.
    // TODO THOMAS This entire thing has to be redone (as specified by Beau)
    public class RoleStore : IRoleStore<SwabbrIdentityRole>
    {
        protected static readonly List<SwabbrIdentityRole> Roles = new List<SwabbrIdentityRole>
        {
            new SwabbrIdentityRole{ RoleId = "0", Name = "User", NormalizedName = "USER" },
            new SwabbrIdentityRole{ RoleId = "1", Name = "Admin", NormalizedName = "ADMIN" },
        };

        public async Task<IdentityResult> CreateAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            //! Not implemented
            // TODO THOMAS This should throw! I suspect this is to enable endpoint calling for iOS and Android? --> yorick: kan met de user manager te maken hebben
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            //! Not implemented
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            //! Not implemented
            return IdentityResult.Success;
        }

        public async Task<SwabbrIdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Roles.Where(r => r.RoleId == roleId).First();
        }

        public async Task<SwabbrIdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Roles.Where(r => r.NormalizedName.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase)).First();
        }

        public async Task<string> GetNormalizedRoleNameAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return role.NormalizedName;
        }

        public async Task<string> GetRoleIdAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return role.RoleId;
        }

        public async Task<string> GetRoleNameAsync(SwabbrIdentityRole role, CancellationToken cancellationToken)
        {
            return role.Name;
        }

        public async Task SetNormalizedRoleNameAsync(SwabbrIdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
        }

        public async Task SetRoleNameAsync(SwabbrIdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
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