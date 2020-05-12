using System.Net.Http;

namespace Swabbr.Core.Interfaces.Factories
{

    public interface IHttpClientFactory
    {

        HttpClient GetClient();

    }

}
