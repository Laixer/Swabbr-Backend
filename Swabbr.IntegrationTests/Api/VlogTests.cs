//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Xunit;

//namespace Swabbr.IntegrationTests.Api
//{
//    /// <summary>
//    ///     Integration tests for the vlog controller.
//    /// </summary>
//    public class VlogTests : IClassFixture<ApiAuthWebApplicationFactory>
//    {
//        private readonly HttpClient _client;

//        /// <summary>
//        ///     Create new instance.
//        /// </summary>
//        public VlogTests(ApiAuthWebApplicationFactory factory)
//        {
//            if (factory is null)
//            {
//                throw new ArgumentNullException(nameof(factory));
//            }

//            _client = factory.CreateClient();
//        }

//        [Fact]
//        public async Task AddViewReturnsNoContent()
//        {
//            // Arrange.
//            var id = Guid.NewGuid();

//            // Act.
//            var responseObject = await _client.PostAsJsonAsync($"api/vlog/{id}/add-view", new { });

//            if (responseObject.StatusCode == HttpStatusCode.Unauthorized)
//            {

//            }

//            // Assert.
//            Assert.Equal(HttpStatusCode.NoContent, responseObject.StatusCode);
//        }
//    }
//}
