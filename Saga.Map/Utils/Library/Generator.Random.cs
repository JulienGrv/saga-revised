using System;

namespace Saga.Utils
{
    internal static class Generator
    {
        private static Random rand = null;

        static Generator()
        {
            rand = new Random();
        }

        public static int Random(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}