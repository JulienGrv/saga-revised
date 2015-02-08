using Saga.Structures;
using System;

namespace Saga.Map.Librairies
{
    public static class Vector
    {
        /// <summary>
        /// Internally used to generate random values.
        /// </summary>
        internal static Random rand = new Random();

        /// <summary>
        /// Calculate the distance between point A and B in three-dimensional space.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static double GetDistance3D(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return Math.Abs(distance);
        }

        /// <summary>
        /// Calculate the distance between poont A and B in two-dimensional space.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static double GetDistance2D(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return Math.Abs(distance);
        }

        public static float[] GetRandomPos2(float x, float y, float range)
        {
            float[] pos = new float[3];
            double degrees = Convert.ToDouble(rand.Next(0, 360));
            double angle = Math.PI * degrees / 180.0;
            double sinAngle = Math.Sin(angle);
            double cosAngle = Math.Cos(angle);

            pos[0] = x + Convert.ToSingle(range * cosAngle);
            pos[1] = y + Convert.ToSingle(range * sinAngle);
            pos[2] = 4000f;

            return pos;
        }

        [Obsolete("Old saga", false)]
        public static float[] GetRandomPos(float x, float y, float range)
        {
            float[] pos = new float[3];
            float[] unitvec = new float[3];
            pos[0] = x;
            pos[1] = y;
            unitvec = GetRandomUnitVector();
            pos = Add(pos, ScalarProduct(unitvec, Convert.ToSingle(rand.Next(0, (int)range))));
            pos[2] = 4000f;
            return pos;
        }

        [Obsolete("Old saga", false)]
        public static float[] GetRandomUnitVector()
        {
            float[] univec = new float[3];
            int rand = new Random().Next(0, 360);

            if (rand % 90 == 0)
            {
                switch (rand)
                {
                    case 0: univec[1] = 1; break;
                    case 90: univec[0] = 1; break;
                    case 180: univec[1] = -1; break;
                    case 270: univec[0] = -1; break;
                }
            }
            else
            {
                if (rand < 180) univec[1] = 1; else univec[1] = -1;
                univec[0] = (float)Math.Tan((rand * 3.14f) / 180);
                univec = GetUnitVector(new float[3] { 0, 0, 0 }, univec);
            }
            return univec;
        }

        [Obsolete("Old saga", false)]
        public static float[] GetUnitVector(float[] src, float[] dst)
        {
            float[] diff = new float[3];
            float distance = GetDistance(src, dst);
            diff[0] = (dst[0] - src[0]) / distance;
            diff[1] = (dst[1] - src[1]) / distance;
            diff[2] = (dst[2] - src[2]) / distance;
            return diff;
        }

        [Obsolete("Old saga", false)]
        public static float GetDistance(float[] src, float[] dst)
        {
            return (float)Math.Sqrt(Pow((dst[0] - src[0]), 2) + Pow((dst[1] - src[1]), 2) + Pow((dst[2] - src[2]), 2));
        }

        [Obsolete("Old saga", false)]
        public static float Pow(float x, int y)
        {
            float tmp = 1f;
            for (int i = 0; i < y; i++)
            {
                tmp = tmp * x;
            }
            return tmp;
        }

        [Obsolete("Old saga", false)]
        public static float[] ScalarProduct(float[] src, float scalar)
        {
            float[] res = new float[3];
            res[0] = src[0] * scalar;
            res[1] = src[1] * scalar;
            res[2] = src[2] * scalar;
            return res;
        }

        [Obsolete("Old saga", false)]
        public static int[] ScalarProduct(int[] src, int scalar)
        {
            int[] res = new int[3];
            res[0] = (int)(src[0] * scalar);
            res[1] = (int)(src[1] * scalar);
            res[2] = (int)(src[2] * scalar);
            return res;
        }

        [Obsolete("Old saga", false)]
        public static float[] Add(float[] v1, float[] v2)
        {
            float[] dst = new float[3];
            dst[0] = v1[0] + v2[0];
            dst[1] = v1[1] + v2[1];
            dst[2] = v1[2] + v2[2];
            return dst;
        }
    }
}