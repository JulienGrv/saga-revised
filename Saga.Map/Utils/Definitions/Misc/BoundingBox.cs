using Saga.Structures;

namespace Saga
{
    /// <summary>
    /// Creates a bouding box used to check collisions.
    /// </summary>
    /// <remarks>
    /// This bounding box pre-transforms it's coords by it's own rotation. This assures
    /// we will align our box model on a horizontal/vertical axis. When checking of a
    /// character is in the box we rotate the characters world coords with the box's
    /// rotation.
    ///
    /// <![CDATA[
    /// When doing this we can simple check if x > point && point < x &&
    /// y > point && point < y.]]>
    ///
    /// Best practice is to precache this box for a prefixed position and check
    /// if character x is in the position.
    /// </remarks>
    public class BoundingBox
    {
        #region Private Members

        /// <summary>
        /// Minimal x coord
        /// </summary>
        private float min_x = 0;

        /// <summary>
        /// Minimal y coord
        /// </summary>
        private float min_y = 0;

        /// <summary>
        /// Maximal x coord
        /// </summary>
        private float max_x = 0;

        /// <summary>
        /// Maximal y coord
        /// </summary>
        private float max_y = 0;

        /// <summary>
        /// Minimal z coord
        /// </summary>
        private float min_z = 0;

        /// <summary>
        /// Maximal z coord
        /// </summary>
        private float max_z = 0;

        /// <summary>
        /// Yaw also known as direction
        /// </summary>
        private ushort yaw = 0;

        #endregion Private Members

        #region Constructor/Deconstructor

        /// <summary>
        /// Creates the bouding box
        /// </summary>
        /// <param name="width">Width of the bounding box (x-axis)</param>
        /// <param name="height">Height of the bouding box (z-axis)</param>
        /// <param name="size">Size of the bouding box (y-axis)</param>
        /// <param name="origin">Center point of the box, size,height,width will be added.</param>
        /// <param name="yaw">Yaw/Rotation of the object</param>
        public BoundingBox(float width, float height, float size, Point origin, ushort yaw)
        {
            //Rotate the bounding box to an alternative grid
            Matrix m = Matrix.GetFromInverseRotation(yaw);
            Point b = Matrix.WorldTransform(origin, m);

            //Create the bounding box
            Point a = new Point(width, height, size);
            TransformationOrigin(a);
            AddWorldTransformPoint(b);

            //Save yaw for later calculations
            this.yaw = yaw;
        }

        #endregion Constructor/Deconstructor

        #region Private Methods

        private void TransformationOrigin(Point b)
        {
            max_x = b.x / 2;
            max_y = b.y / 2;
            max_z = b.z / 2;
            min_x = -max_x;
            min_y = -max_y;
            min_z = -max_z;
        }

        private void AddWorldTransformPoint(Point b)
        {
            min_x += b.x;
            min_y += b.y;
            min_z += b.z;
            max_x += b.x;
            max_y += b.y;
            max_z += b.z;
        }

        private bool BetweenCoordX(float x)
        {
            return x >= min_x && x <= max_x;
        }

        private bool BetweenCoordY(float y)
        {
            return y >= min_y && y <= max_y;
        }

        private bool BetweenCoordZ(float z)
        {
            return z >= min_z && z <= max_z;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Check if the point is in the box. This function assumes
        /// the point hasn't been prerotated yet.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInBox(Point point)
        {
            //Rotate the bounding box to an alternative grid
            Matrix m = Matrix.GetFromInverseRotation(yaw);
            Point b = Matrix.WorldTransform(point, m);

            if (BetweenCoordX(b.x) && BetweenCoordY(b.y) && BetweenCoordZ(b.z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Public Methods
    }
}