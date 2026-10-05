using LinkShortener.Services;

namespace LinkShortener.Tests.Fakes
{
    public class FakeSlugGenerator(params string[] slugs) : ISlugGenerator
    {
        private readonly Queue<string> _slugs = new(slugs);

        public int CallCount { get; private set; }

        public string Generate()
        {
            CallCount++;
            return _slugs.Dequeue();
        }
    }
}
