using LinkShortener.Services;

namespace LinkShortener.Tests.Services
{
    public class RandomSlugGeneratorTests
    {
        [Fact]
        public void Generate_ReturnsSevenCharacterAlphanumericSlug()
        {
            RandomSlugGenerator generator = new();

            string slug = generator.Generate();

            Assert.Matches("^[a-zA-Z0-9]{7}$", slug);
        }
    }
}
