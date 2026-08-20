using System.ComponentModel.DataAnnotations;

namespace LinkShortener.Dtos
{
    public class CreateLinkRequest
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
