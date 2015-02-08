using System;
using System.Globalization;

namespace Saga.Structures
{
    /// <summary>
    /// Basic structure used for x,y,z coords.
    /// </summary>
    [Serializable()]
    public struct Point
    {
        #region Public Members

        /// <summary>
        /// x-component
        /// </summary>
        public float x;

        /// <summary>
        /// y-component
        /// </summary>
        public float y;

        /// <summary>
        /// z-component
        /// </summary>
        public float z;

        #endregion Public Members

        #region Constructors / Deconstructors

        /// <summary>
        /// Initializes a point object out of 3 supplied arguments.
        /// </summary>
        /// <param name="x">Position on the x-axis</param>
        /// <param name="y">Position on the y-axis</param>
        /// <param name="z">Position on the z-axis</param>
        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Initialized a position object out of a array of floats.
        /// </summary>
        /// <remarks>
        /// The first array element serves as the x component. The second array member
        /// serves a the y component. The third element serves as the z component.
        /// </remarks>
        /// <param name="pos">Array of 3 floats.</param>
        public Point(float[] pos)
        {
            this.x = pos[0];
            this.y = pos[1];
            this.z = pos[2];
        }

        #endregion Constructors / Deconstructors

        #region Operator Overloading

        /// <summary>
        /// Substracts one point with another
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Substracted point</returns>
        public static Point operator -(Point c1, Point c2)
        {
            Point temp = new Point();
            temp.x = c1.x - c2.x;
            temp.y = c1.y - c2.y;
            temp.z = c1.z - c2.z;
            return temp;
        }

        /// <summary>
        /// Adds one point with another
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Added point</returns>
        public static Point operator +(Point c1, Point c2)
        {
            Point temp = new Point();
            temp.x = c1.x + c2.x;
            temp.y = c1.y + c2.y;
            temp.z = c1.z + c2.z;
            return temp;
        }

        /// <summary>
        /// Multiplies one point with another
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Multiplied point</returns>
        public static Point operator *(Point c1, Point c2)
        {
            Point temp = new Point();
            temp.x = c1.x * c2.x;
            temp.y = c1.y * c2.y;
            temp.z = c1.z * c2.z;
            return temp;
        }

        /// <summary>
        /// Divides one point with another
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Divided point</returns>
        public static Point operator /(Point c1, Point c2)
        {
            Point temp = new Point();
            temp.x = c1.x / c2.x;
            temp.y = c1.y / c2.y;
            temp.z = c1.z / c2.z;
            return temp;
        }

        /// <summary>
        /// Compares if point a is the same as point b
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Divided point</returns>
        public static bool operator ==(Point c1, Point c2)
        {
            return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
        }

        /// <summary>
        /// Compares if point a is not the same as point b
        /// </summary>
        /// <param name="c1">Left assignment point</param>
        /// <param name="c2">Right assignment point</param>
        /// <returns>Divided point</returns>
        public static bool operator !=(Point c1, Point c2)
        {
            return !(c1.x == c2.x && c1.y == c2.y && c1.z == c2.z);
        }

        public override bool Equals(object obj)
        {
            try
            {
                Point p = (Point)obj;
                return p.x == this.x && p.y == this.y && p.z == z;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)(x + y + z);
        }

        #endregion Operator Overloading

        #region Public Methods

        /// <summary>
        /// Represents the point in a human readable format
        /// </summary>
        /// <returns>A human readable string</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", x, y, z);
        }

        #endregion Public Methods

        #region Extensions

        private const double SightRangeRadius = 4048;
        private const double SightRangeRadius3D = SightRangeRadius * SightRangeRadius;

        /// <summary>
        /// Check if a object is in sightrange of eachother
        /// </summary>
        /// <returns>
        /// Checks if position a is in a range of position b.
        /// This sub function is used to calculate objects we are allowed to
        /// see. Applied for 3 axices: X, Y, Z.///
        /// </returns>
        public static bool IsInSightRangeByRadius(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = dx * dx + dy * dy + dz * dz;
            return distance < SightRangeRadius3D;
        }

        [Obsolete("Point.IsInSightRangeByRadius is obsolete use IsInSightRangeByRadiusUnsquared preferable", false)]
        public static bool IsInSightRangeByRadius(Point A, Point B, uint maxdistance)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = dx * dx + dy * dy + dz * dz;
            return Math.Sqrt(distance) < maxdistance;
        }

        public static bool IsInSightRangeByRadiusUnsquared(Point A, Point B, uint maxdistance)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = dx * dx + dy * dy + dz * dz;
            return distance < maxdistance;
        }

        public static double GetDistance3D(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return distance;
        }

        public static double GetNonSquaredDistance3D(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = dx * dx + dy * dy + dz * dz;
            return distance;
        }

        public static ushort CalculateYaw(Point a)
        {
            return (ushort)(Math.Atan2(a.y, a.x) * 32768 / Math.PI);
        }

        public static ushort CalculateYaw(Point a, Point b)
        {
            return (ushort)(Math.Atan2(b.y - a.y, b.x - a.x) * 32768 / Math.PI);
        }

        #endregion Extensions
    }
}