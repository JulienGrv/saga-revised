using Saga.Structures;
using System;
using System.Diagnostics;

namespace Saga
{
    /// <summary>
    /// Matrix class for doing matrix conversions.
    /// </summary>
    public class Matrix
    {
        #region Protected Members

        /// <summary>
        /// 2 dimensional array containing the matrix values
        /// </summary>
        protected float[,] matrix = new float[3, 3];

        #endregion Protected Members

        #region Public Static Methods

        /// <summary>
        /// Get's a rotation matrix from the specified yaw.
        /// </summary>
        /// <param name="yaw">Yaw for the matrix</param>
        /// <returns>Matrix useable for rotations</returns>
        public static Matrix GetFromRotation(ushort yaw)
        {
            double rad = Saga.Structures.Yaw.ToRadiants(yaw);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Matrix matrix = new Matrix();
            matrix.matrix[0, 0] = cos;
            matrix.matrix[0, 1] = -sin;
            matrix.matrix[1, 0] = sin;
            matrix.matrix[1, 1] = cos;
            matrix.matrix[2, 2] = 1;
            return matrix;
        }

        /// <summary>
        /// Get's a inversed rotation matrix from the specified yaw.
        /// </summary>
        /// <param name="yaw">Yaw for the matrix</param>
        /// <returns>Matrix useable for rotations</returns>
        /// <remarks>
        /// This flips the x and the y axises.
        /// </remarks>
        public static Matrix GetFromInverseRotation(ushort yaw)
        {
            double rad = Saga.Structures.Yaw.ToRadiants(yaw);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            Matrix matrix = new Matrix();
            matrix.matrix[0, 0] = sin;
            matrix.matrix[0, 1] = cos;
            matrix.matrix[1, 0] = -cos;
            matrix.matrix[1, 1] = sin;
            matrix.matrix[2, 2] = 1;
            return matrix;
        }

        /// <summary>
        /// Performs a world coord translation with the specified
        /// rotation matrix.
        /// </summary>
        /// <remarks>
        /// The outputed point will be rotated against it's center point of the world
        /// beeing 0,0,0 (xyz). The outputed coords will keep the same vector distance
        /// as the input coord.
        /// </remarks>
        /// <param name="position">Point to rotate</param>
        /// <param name="matrix">Matrix used for the rotation</param>
        /// <returns>Transformed point</returns>
        public static Point WorldTransform(Point position, Matrix matrix)
        {
            Point temp = new Point();
            temp.x = position.x * matrix[0, 0] + position.y * matrix[1, 0] + position.z * matrix[2, 0];
            temp.y = position.y * matrix[0, 1] + position.y * matrix[1, 1] + position.z * matrix[2, 1];
            temp.z = position.x * matrix[0, 2] + position.y * matrix[1, 2] + position.z * matrix[2, 2];
            return temp;
        }

        #endregion Public Static Methods

        #region Public Properties

        /// <summary>
        /// Get's the the value of the matrix argument
        /// </summary>
        /// <param name="index"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        [DebuggerNonUserCode()]
        public float this[int index, int index2]
        {
            get
            {
                return matrix[index, index2];
            }
        }

        #endregion Public Properties
    }
}