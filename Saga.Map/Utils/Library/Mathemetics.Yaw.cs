using System;

namespace Saga.Library.Mathemetics
{
    internal static class Yaw
    {
        /// <summary>
        /// Converts yaw to gradians
        /// </summary>
        /// <param name="yaw"></param>
        /// <returns></returns>
        /// <remarks>
        /// This converts Yaw to degrees
        ///
        /// yaw is specified as the maximum value of a ushort
        /// which is 65535. Therefor Degrees = Yaw / Max_Yaw * Max_degrees
        /// </remarks>
        public static int YawToDegrees(int yaw)
        {
            float float_yaw = Convert.ToSingle(yaw) % 65535F;
            float_yaw /= 65535F;
            float_yaw *= 360F;
            return Convert.ToInt32(float_yaw);
        }

        /// <summary>
        /// Converts gradians to yaw
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        /// <remarks>
        /// This converts Degrees to Yaw
        ///
        /// Degrees are specified with the maximum value of 360.
        /// Therefor Yaw = Degrees / Max_Degrees * Max_Yaw;
        /// </remarks>
        public static int YawFromDegrees(int degrees)
        {
            float float_degrees = Convert.ToSingle(degrees) % 360F;
            float_degrees /= 360F;
            float_degrees *= 65535F;
            return Convert.ToInt32(float_degrees);
        }

        /// <summary>
        /// Checks if Yaw is in range of
        /// </summary>
        /// <param name="yaw1"></param>
        /// <param name="yaw2"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function checks if Yaw 1 is in range of Yaw 2 by the range
        /// measured in yaw units.
        /// </remarks>
        public static bool IsYawInRangeOf(int yaw1, int yaw2, int range)
        {
            unchecked
            {
                ushort y = (ushort)yaw1;
                ushort x = (ushort)yaw2;
                return y > (ushort)(x - range) && y < (ushort)(x + range);
            }
        }
    }
}