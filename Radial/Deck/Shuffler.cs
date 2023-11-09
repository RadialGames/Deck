using System.Collections.Generic;
using Random = System.Random;

namespace Radial.Deck
{
    public static class Shuffler
    {
        private static Random random = new Random();

        public static void SetRandomSeed(int seed)
        {
            random = new Random(seed);
        }

        public static T[] Shuffle<T>(T[] items)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                int pos = random.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[pos];
                items[pos] = temp;
            }

            return items;
        }

        public static IList<T> Shuffle<T>(IList<T> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                int pos = random.Next(i, items.Count);
                T temp = items[i];
                items[i] = items[pos];
                items[pos] = temp;
            }

            return items;
        }

        public static float RandomFloatInRange(float min, float max)
        {
            return (float) random.NextDouble() * (max - min) + min;
        }
    }
}