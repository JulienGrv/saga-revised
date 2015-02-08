using System;
using System.Text;

namespace Saga.Network
{
    public class SpecialEncoding
    {
        public static string Read(ref byte[] data, int index, int count)
        {
            int max = Math.Max(data.Length, index + count);
            int length = max - index;
            int sub = index % 2;

            for (int i = index; i < max; i++)
            {
                if (data[i] == 0 && data[i + 1] == 0 && (i - sub) % 2 == 0)
                {
                    length = i - index;
                    break;
                }
            }
            return Encoding.Unicode.GetString(data, index, length);
        }
    }
}