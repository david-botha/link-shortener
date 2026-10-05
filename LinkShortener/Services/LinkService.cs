using LinkShortener.Data;
using LinkShortener.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Services
{
    public class LinkService(AppDbContext db, ISlugGenerator slugGenerator) : ILinkService
    {
        private const int MaxAttempts = 5;

        public async Task<Link?> CreateAsync(string originalUrl)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Link link = new()
                {
                    Slug = slugGenerator.Generate(),
                    OriginalUrl = originalUrl,
                    CreatedAt = DateTime.UtcNow
                };

                db.Links.Add(link);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    db.Entry(link).State = EntityState.Detached;
                    continue;
                }

                return link;
            }

            return null;
        }

        public async Task<string?> GetOriginalUrlAsync(string slug)
        {
            return await db.Links
                .Where(link => link.Slug == slug)
                .Select(link => link.OriginalUrl)
                .FirstOrDefaultAsync();
        }
    }
}
