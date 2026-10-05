using LinkShortener.Data;
using LinkShortener.Models;
using LinkShortener.Services;
using LinkShortener.Tests.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Tests.Services
{
    public class LinkServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly AppDbContext _db;
        private readonly LinkService _service;

        public LinkServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _db = new AppDbContext(options);
            _db.Database.EnsureCreated();

            _service = new LinkService(_db, new RandomSlugGenerator());
        }

        public void Dispose()
        {
            _db.Dispose();
            _connection.Dispose();
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ReturnsOriginalUrl_ForCreatedLink()
        {
            string originalUrl = "https://example.com";
            Link? created = await _service.CreateAsync(originalUrl);
            Assert.NotNull(created);

            string? foundUrl = await _service.GetOriginalUrlAsync(created.Slug);

            Assert.Equal(originalUrl, foundUrl);
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ReturnsNull_ForUnknownSlug()
        {
            string? foundUrl = await _service.GetOriginalUrlAsync("missing");

            Assert.Null(foundUrl);
        }

        [Fact]
        public async Task CreateAsync_RetriesWithNewSlug_WhenSlugIsTaken()
        {
            _db.Links.Add(new Link { Slug = "taken01", OriginalUrl = "https://example.com/existing" });
            await _db.SaveChangesAsync(TestContext.Current.CancellationToken);
            LinkService service = new(_db, new FakeSlugGenerator("taken01", "fresh01"));

            Link? created = await service.CreateAsync("https://example.com/new");

            Assert.NotNull(created);
            Assert.Equal("fresh01", created.Slug);
            Assert.Equal("https://example.com/new", await service.GetOriginalUrlAsync("fresh01"));
        }

        [Fact]
        public async Task CreateAsync_ReturnsNull_WhenEverySlugIsTaken()
        {
            _db.Links.Add(new Link { Slug = "taken01", OriginalUrl = "https://example.com/existing" });
            await _db.SaveChangesAsync(TestContext.Current.CancellationToken);
            FakeSlugGenerator slugGenerator = new("taken01", "taken01", "taken01", "taken01", "taken01");
            LinkService service = new(_db, slugGenerator);

            Link? created = await service.CreateAsync("https://example.com/new");

            Assert.Null(created);
            Assert.Equal(5, slugGenerator.CallCount);
        }
    }
}
