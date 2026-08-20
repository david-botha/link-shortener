using LinkShortener.Data;
using LinkShortener.Dtos;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LinkShortener.Controllers
{
    [ApiController]
    [Route("api/links")]
    public class LinksController(AppDbContext db) : ControllerBase
    {
        private const string SlugAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int SlugLength = 7;
        private const int MaxAttempts = 5;

        [HttpPost]
        public async Task<ActionResult<CreateLinkResponse>> Create(CreateLinkRequest request)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Link link = new()
                {
                    Slug = GenerateSlug(),
                    OriginalUrl = request.OriginalUrl,
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

                CreateLinkResponse response = new()
                {
                    Slug = link.Slug,
                    ShortUrl = $"{Request.Scheme}://{Request.Host}/{link.Slug}",
                    OriginalUrl = link.OriginalUrl,
                    CreatedAt = link.CreatedAt
                };

                return Created(response.ShortUrl, response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Could not generate a unique slug.");
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
