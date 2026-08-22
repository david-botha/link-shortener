using LinkShortener.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Controllers
{
    [ApiController]
    public class RedirectController(AppDbContext db) : ControllerBase
    {
        // [[ ]] are escaped [ ]. Actual pattern: ^[a-zA-Z0-9_-]+$
        [HttpGet("{slug:regex(^[[a-zA-Z0-9_-]]+$)}")]
        public async Task<IActionResult> RedirectToOriginal(string slug)
        {
            string? originalUrl = await db.Links
                .Where(link => link.Slug == slug)
                .Select(link => link.OriginalUrl)
                .FirstOrDefaultAsync();

            if (originalUrl is null)
            {
                return NotFound();
            }

            return Redirect(originalUrl);
        }
    }
}
