using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="Request"/> repository.
    /// </summary>
    public interface IRequestRepository : ICudFunctionality<Request, Guid>
    {

        Task<RequestRecordVlog> GetAsync(Guid requestId);

        Task<Request> MarkAsync(Guid requestId, RequestState state);

    }

}
