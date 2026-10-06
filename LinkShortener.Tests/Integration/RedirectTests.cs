using System.Net;
using System.Net.Http.Json;
using LinkShortener.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LinkShortener.Tests.Integration
{
    public class RedirectTests(LinkShortenerFactory factory) : IClassFixture<LinkShortenerFactory>
    {
        private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        [Fact]
        public async Task GetSlug_Redirects302_ToOriginalUrl()
        {
            string originalUrl = "https://example.com/docs?page=2";
            HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/links", new CreateLinkRequest { OriginalUrl = originalUrl }, TestContext.Current.CancellationToken);
            CreateLinkResponse? created = await createResponse.Content.ReadFromJsonAsync<CreateLinkResponse>(TestContext.Current.CancellationToken);
            Assert.NotNull(created);

            HttpResponseMessage response = await _client.GetAsync($"/{created.Slug}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.Equal(originalUrl, response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task GetSlug_Returns404_ForUnknownSlug()
        {
            HttpResponseMessage response = await _client.GetAsync("/missing1", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
