namespace LinkShortener.Models;

public class Link
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string OriginalUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
