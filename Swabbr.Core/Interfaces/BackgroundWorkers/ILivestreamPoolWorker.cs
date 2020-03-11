using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.BackgroundWorkers
{

    /// <summary>
    /// Contains functionality to control a <see cref="Entities.Livestream"/>
    /// pool.
    /// </summary>
    public interface ILivestreamPoolWorker
    {

        Task UpdatePoolAsync();

    }

}
