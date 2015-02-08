using System;

namespace Saga.Shared.PacketLib.Other
{
    public static class Conversions
    {
        private static char[] HEXDIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static string ByteToHexString(byte[] value)
        {
            char[] aChars = new char[value.Length * 2];
            for (int i = 0; i < value.Length; ++i)
            {
                int iByte = value[i];
                aChars[i * 2] = HEXDIGITS[iByte >> 4];
                aChars[i * 2 + 1] = HEXDIGITS[iByte & 0xF];
            }
            return new string(aChars);
        }

        public static byte[] HexStringToBytes(string value)
        {
            byte[] aBytes = new byte[value.Length / 2];
            int offset = 0;
            for (int i = 0; i < value.Length; i++)
            {
                string tmp = "";
                tmp += value[i];
                tmp += value[i + 1];
                aBytes[offset] = byte.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                offset++;
                i++;
            }
            return aBytes;
        }

        public static bool TryParse(string value, out byte[] key)
        {
            try
            {
                key = HexStringToBytes(value);
                return true;
            }
            catch (Exception)
            {
                key = null;
                return false;
            }
        }
    }
}