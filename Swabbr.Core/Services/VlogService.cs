using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public class VlogService : IVlogService
    {
        private readonly IVlogRepository _vlogRepository;

        public VlogService(IVlogRepository vlogRepository)
        {
            _vlogRepository = vlogRepository;
        }

        public Task<bool> ExistsAsync(Guid vlogId)
        {
            return _vlogRepository.ExistsAsync(vlogId);
        }

        public Task<Vlog> GetAsync(Guid vlogId)
        {
            return _vlogRepository.GetByIdAsync(vlogId);
        }

        public Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync()
        {
            return _vlogRepository.GetFeaturedVlogsAsync();
        }

        public Task<IEnumerable<Guid>> GetSharedUsersAsync(Guid vlogId)
        {
            //!IMPORTANT
            //TODO: Implement function
            throw new NotImplementedException();
        }

        public Task<int> GetVlogCountForUserAsync(Guid userId)
        {
            return _vlogRepository.GetVlogCountForUserAsync(userId);
        }

        public Task<IEnumerable<Vlog>> GetVlogsByUserAsync(Guid userId)
        {
            return _vlogRepository.GetVlogsByUserAsync(userId);
        }

        public async Task ShareWithUserAsync(Guid vlogId, Guid userId)
        {
            await _vlogRepository.ShareWithUserAsync(vlogId, userId);
        }
    }
}
