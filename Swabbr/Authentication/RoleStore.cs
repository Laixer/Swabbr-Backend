using Microsoft.AspNetCore.Identity;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Authentication
{
    public class RoleStore : IRoleStore<Role>
    {
        private readonly IRoleRepository _repository;

        public RoleStore(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            //TODO
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await _repository.GetAsync(roleId, roleId);
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            // TODO
            return new Role();
        }

        public async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            // TODO
            return "x";
        }

        public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken) => role.RoleId.ToString();

        public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken) => role.Name.ToString();

        public async Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            // TODO
        }

        public async Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            // TODO
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(role);
            return IdentityResult.Success;
        }
    }
}
