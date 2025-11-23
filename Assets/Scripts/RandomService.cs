namespace Core
{
    public static class RandomService
    {
        private static System.Random _random = new(12);

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