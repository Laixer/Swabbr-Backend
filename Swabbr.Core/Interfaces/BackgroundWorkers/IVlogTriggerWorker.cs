using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.BackgroundWorkers
{

    /// <summary>
    /// Contract for a background worker that manages when a user should get a
    /// vlog record request.
    /// </summary>
    public interface IVlogTriggerWorker
    {

        Task SendAllRequestsAsync();

    }

}
