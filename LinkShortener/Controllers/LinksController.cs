using LinkShortener.Dtos;
using LinkShortener.Models;
using LinkShortener.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Controllers
{
    [ApiController]
    [Route("api/links")]
    public class LinksController(ILinkService linkService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<CreateLinkResponse>> Create(CreateLinkRequest request)
        {
            Link? link = await linkService.CreateAsync(request.OriginalUrl);

            if (link is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not generate a unique slug.");
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
    }
}
