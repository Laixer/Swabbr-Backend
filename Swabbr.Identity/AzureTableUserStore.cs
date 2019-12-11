using Microsoft.AspNet.Identity; //TODO Core Library
using Swabbr.Infrastructure.Data.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Identity
{
    public class AzureTableUserStore : IUserStore<AzureTableIdentityUser>
    {
        private readonly IDbClient<AzureTableIdentityUser> _dbClient;

        public AzureTableUserStore(IDbClientFactory factory)
        {
            _dbClient = factory.GetClient<AzureTableIdentityUser>("User");
        }

        public async Task CreateAsync(AzureTableIdentityUser user)
        {
            await _dbClient.InsertEntityAsync(user);
        }

        public Task DeleteAsync(AzureTableIdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<AzureTableIdentityUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AzureTableIdentityUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AzureTableIdentityUser user)
        {
            throw new NotImplementedException();
        }

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
        // ~AzureTableUserStore()
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
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
