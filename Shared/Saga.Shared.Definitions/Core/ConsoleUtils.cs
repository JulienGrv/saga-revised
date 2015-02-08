using System;
using System.Text;

public static class ConsoleUtils
{
    #region Console.Progressbar

    public static void ProgressBarShow(uint progressPos, uint progressTotal, string label)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("\r{0} [", label);
        uint barPos = progressPos * 40 / progressTotal + 1;
        for (uint p = 0; p < barPos; p++) sb.AppendFormat("#");
        for (uint p = barPos; p < 40; p++) sb.AppendFormat(" ");
        sb.AppendFormat("] {0}%\r", progressPos * 100 / progressTotal);
        Console.Write(sb.ToString());
    }

    public static void ProgressBarHide(string label)
    {
        char[] buffer = new char[80];
        label.CopyTo(0, buffer, 0, label.Length);
        Console.Write(buffer);
    }

    #endregion Console.Progressbar

    #region Array Parsing

    public static byte[] ParseToByteArray(string value)
    {
        string[] values = value.Split(',');
        byte[] bytes = new byte[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            bytes[i] = byte.Parse(values[i], System.Globalization.CultureInfo.InvariantCulture);
        }
        return bytes;
    }

    public static uint[] ParseToUintArray(string value)
    {
        string[] values = value.Split(',');
        uint[] uints = new uint[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            uints[i] = uint.Parse(values[i], System.Globalization.CultureInfo.InvariantCulture);
        }
        return uints;
    }

    #endregion Array Parsing
}