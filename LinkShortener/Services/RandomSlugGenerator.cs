using System.Security.Cryptography;

namespace LinkShortener.Services
{
    public class RandomSlugGenerator : ISlugGenerator
    {
        private const string SlugAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int SlugLength = 7;

        public string Generate()
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
