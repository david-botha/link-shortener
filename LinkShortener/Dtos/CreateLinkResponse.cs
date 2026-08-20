namespace LinkShortener.Dtos
{
    public class CreateLinkResponse
    {
        public string Slug { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
