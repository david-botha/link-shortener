using LinkShortener.Models;

namespace LinkShortener.Services
{
    public interface ILinkService
    {
        Task<Link?> CreateAsync(string originalUrl);
        Task<string?> GetOriginalUrlAsync(string slug);
    }
}
