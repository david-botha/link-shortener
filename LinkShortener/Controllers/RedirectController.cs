using LinkShortener.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Controllers
{
    [ApiController]
    public class RedirectController(ILinkService linkService) : ControllerBase
    {
        // [[ ]] are escaped [ ]. Actual pattern: ^[a-zA-Z0-9_-]+$
        [HttpGet("{slug:regex(^[[a-zA-Z0-9_-]]+$)}")]
        public async Task<IActionResult> RedirectToOriginal(string slug)
        {
            string? originalUrl = await linkService.GetOriginalUrlAsync(slug);

            if (originalUrl is null)
            {
                return NotFound();
            }

            return Redirect(originalUrl);
        }
    }
}
