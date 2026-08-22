using LinkShortener.Data;
using LinkShortener.Dtos;
using LinkShortener.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LinkShortener.Services
{
    public class LinkService(AppDbContext db) : ILinkService
    {
        private const string SlugAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int SlugLength = 7;
        private const int MaxAttempts = 5;

        public async Task<Link?> CreateAsync(string originalUrl)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Link link = new()
                {
                    Slug = GenerateSlug(),
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

        private static string GenerateSlug()
        {
            char[] chars = new char[SlugLength];

            for (int i = 0; i < SlugLength; i++)
            {
                chars[i] = SlugAlphabet[RandomNumberGenerator.GetInt32(SlugAlphabet.Length)];
            }

            return new string(chars);
        }
    }
}
