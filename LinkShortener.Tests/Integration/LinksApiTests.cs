using System.Net;
using System.Net.Http.Json;
using LinkShortener.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Tests.Integration
{
    public class LinksApiTests(LinkShortenerFactory factory) : IClassFixture<LinkShortenerFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task PostLink_Returns201_WithLocationOfShortUrl()
        {
            CreateLinkRequest request = new() { OriginalUrl = "https://example.com" };

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/links", request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            CreateLinkResponse? body = await response.Content.ReadFromJsonAsync<CreateLinkResponse>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(body);
            Assert.Equal("https://example.com", body.OriginalUrl);
            Assert.Equal(body.ShortUrl, response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task PostLink_Returns400_ForInvalidUrl()
        {
            CreateLinkRequest request = new() { OriginalUrl = "not a url" };

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/links", request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            ValidationProblemDetails? problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains(nameof(CreateLinkRequest.OriginalUrl), problem.Errors.Keys);
        }
    }
}
