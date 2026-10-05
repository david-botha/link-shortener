using LinkShortener.Data;
using LinkShortener.Models;
using LinkShortener.Services;
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

            _service = new LinkService(_db);
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
    }
}
