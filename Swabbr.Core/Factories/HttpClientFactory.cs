using Swabbr.Core.Interfaces.Factories;
using System.Net.Http;

namespace Swabbr.Core.Factories
{

    /// <summary>
    /// Generates <see cref="HttpClient"/>s for us.
    /// TODO Is this safe? Maybe using singleton of asp?
    /// </summary>
    public sealed class HttpClientFactory : IHttpClientFactory
    {

        private static readonly HttpClient client = new HttpClient();

        public HttpClient GetClient()
        {
            return client;
        }

    }

}
