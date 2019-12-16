using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync();
    }
}