using System;

namespace Core
{
    /// <summary>
    /// Deterministik, seed tabanlı random servisi.
    /// Level başında seed verilerek oyun tamamen replay edilebilir hale gelir.
    /// </summary>
    public static class RandomService
    {
        private static System.Random _random = new System.Random(12345);

        public static void SetSeed(int seed)
        {
            _random = new System.Random(seed);
        }

        public static int Range(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }

            return _random.Next(minInclusive, maxExclusive);
        }
    }
}