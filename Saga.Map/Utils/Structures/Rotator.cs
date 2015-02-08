using System;

namespace Saga.Structures
{
    /// <summary>
    /// Get's the compose yaw rotator
    /// </summary>
    [Serializable()]
    public struct Rotator
    {
        /// <summary>
        /// Ushort value of the rotation
        /// </summary>
        public ushort rotation;

        /// <summary>
        /// Ushort value of a unknown
        /// </summary>
        public ushort unknown;

        /// <summary>
        /// Creates a new rotator
        /// </summary>
        /// <param name="rotation">rotation</param>
        /// <param name="u">uknown vector</param>
        public Rotator(ushort rotation, ushort u)
        {
            this.rotation = rotation;
            this.unknown = u;
        }

        /// <summary>
        /// Creates a new rotator from a signed integer
        /// </summary>
        /// <param name="value">Signed integer</param>
        /// <returns>Rotator</returns>
        public static implicit operator Rotator(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            Rotator rot = new Rotator();
            rot.rotation = BitConverter.ToUInt16(arr, 0);
            rot.unknown = BitConverter.ToUInt16(arr, 2);
            return rot;
        }
    }

    public class Yaw
    {
#if !YAWTORADPRECACHE

        private static double[] radiants;

        static Yaw()
        {
            radiants = new double[665365];
            for (int i = 0; i < 665365; i++)
            {
                radiants[i] = ((double)i * (Math.PI / 32768));
            }
        }

        public static double ToRadiants(ushort yaw)
        {
            return radiants[yaw];
        }

#else

        const double __pi = Math.PI / 32768;
        public static double ToRadiants(ushort yaw)
        {
            return (double)yaw * __pi;
        }

#endif
    }

    public struct WaypointStructure
    {
        public Point point;
        public Rotator rotation;

        public WaypointStructure(Point point, Rotator rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }
    }
}