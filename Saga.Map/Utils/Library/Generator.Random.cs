using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Utils
{
    static class Generator
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
