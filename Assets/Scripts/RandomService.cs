using System;

namespace Core
{
    public static class RandomService
    {
        private static Random _random = new(12345);

        public static void SetSeed(int seed)
        {
            _random = new Random(seed);
        }

        public static int Range(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive) return minInclusive;

            return _random.Next(minInclusive, maxExclusive);
        }
    }
}