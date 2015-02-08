using System;
using System.Diagnostics;
using System.IO;

namespace Saga.Structures
{
    /// <summary>
    /// This class is ported from saga to contain heightmap information
    /// for our maps.
    /// </summary>
    public class HeightMap
    {
        #region Heightmap - Private Members

        // PRIVATE VARIABLES

        /// <summary>
        /// Contains the height in a 2-dimensional array
        /// </summary>
        private float[,] HeightData;

        #endregion Heightmap - Private Members

        #region Heightmap - Public Members

        //PUBLIC VARIABLES

        /// <summary>
        /// This structure is internally used to compute the z-coords by a given
        /// x and y postion.
        /// </summary>
        public HeightMapInfo info;

        //PUBLIC MEMBERS

        /// <summary>
        /// This function returns a random x,y,z position for the given
        /// heightmap.
        /// </summary>
        /// <returns></returns>
        public float[] GetRandomPos()
        {
            float[] ret = new float[3];

            for (int i = 0; i < 10000; i++)
            {
                int max = (int)(((this.info.size / 2) * this.info.scale[0]) + this.info.location[0]);
                int min = (int)((-(this.info.size / 2) * this.info.scale[0]) + this.info.location[0]);

                ret[0] = random.Next(min, max + 1);
                ret[1] = random.Next(min, max + 1);

                this.GetZ(ret[0], ret[1], out ret[2]);

                //if (ret[2] < this.water_level) continue;

                float check;

                this.GetZ(ret[0] + this.info.scale[0], ret[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0] - this.info.scale[0], ret[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0], ret[1] + this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0], ret[1] - this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0] + this.info.scale[0], ret[1] + this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0] + this.info.scale[0], ret[1] - this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0] - this.info.scale[0], ret[1] + this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                this.GetZ(ret[0] - this.info.scale[0], ret[1] - this.info.scale[1], out check);
                if (Math.Abs(check - ret[2]) > 100) continue;

                break;
            }
            return ret;
        }

        /// <summary>
        /// This function computes the z-coord by a given x and y position.
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="z">z position to return</param>
        /// <returns>True/False if the z position was found</returns>
        public bool GetZ(float x, float y, out float z)
        {
            try
            {
                point2D delta;
                point3D point1, point2, point3, point4;

                ushort mx = (ushort)Math.Ceiling((float)((x - this.info.location[0]) / this.info.scale[0]) + (this.info.size / 2));
                ushort my = (ushort)Math.Ceiling((float)((y - this.info.location[1]) / this.info.scale[1]) + (this.info.size / 2));

                if (mx >= this.info.size || my >= this.info.size)
                {
                    z = 0;
                    return false;
                }

                point1.x = (float)((mx - (this.info.size / 2)) * this.info.scale[0]) + this.info.location[0];
                point1.y = (float)((my - (this.info.size / 2)) * this.info.scale[1]) + this.info.location[1];
                point1.z = (float)(this.HeightData[mx, my] * this.info.scale[2]) + this.info.location[2];

                mx -= 1;
                point2.x = (float)((mx - (this.info.size / 2)) * this.info.scale[0]) + this.info.location[0];
                point2.y = (float)((my - (this.info.size / 2)) * this.info.scale[1]) + this.info.location[1];
                point2.z = (float)(this.HeightData[mx, my] * this.info.scale[2]) + this.info.location[2];

                my -= 1;
                point3.x = (float)((mx - (this.info.size / 2)) * this.info.scale[0]) + this.info.location[0];
                point3.y = (float)((my - (this.info.size / 2)) * this.info.scale[1]) + this.info.location[1];
                point3.z = (float)(this.HeightData[mx, my] * this.info.scale[2]) + this.info.location[2];

                mx += 1;
                point4.x = (float)((mx - (this.info.size / 2)) * this.info.scale[0]) + this.info.location[0];
                point4.y = (float)((my - (this.info.size / 2)) * this.info.scale[1]) + this.info.location[1];
                point4.z = (float)(this.HeightData[mx, my] * this.info.scale[2]) + this.info.location[2];

                delta.x = point1.x - x;
                delta.y = point1.y - y;

                if (delta.x >= delta.y) //use 1,2,3
                    z = (float)HeightMap.z(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z, x, y) + z_correction;
                else //use 1,3,4
                    z = (float)HeightMap.z(point1.x, point1.y, point1.z, point3.x, point3.y, point3.z, point4.x, point4.y, point4.z, x, y) + z_correction;
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                z = 0;
                return false;
            }
        }

        #endregion Heightmap - Public Members

        #region Heightmap - Private Static Members

        //PRIVATE VARIABLES

        /// <summary>
        /// Used internally for the z-correction on our calculations.
        /// </summary>
        private static float z_correction = 103.6f;

        /// <summary>
        /// Random generator used to generate a random x and y coord
        /// for the random coord generator.
        /// </summary>
        private static Random random = new Random();

        //PRIVATE METHODS

        /// <summary>
        /// This function calculates the determinate out of the given heightmap.
        /// This will the z-step positions between each milestone of the heightmap by
        /// reading the surrounding coords.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static double det(double a, double b, double c, double d, double e, double f, double g, double h, double i)
        {
            return a * e * i + b * f * g + c * d * h - c * e * g - f * h * a - i * b * d;
        }

        /// <summary>
        /// This calculates the z-coord by using the determinate.
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="b3"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double z(double a1, double a2, double a3, double b1, double b2, double b3, double c1, double c2, double c3, double x, double y)
        {
            return (det(a1, b1, c1, a2, b2, c2, a3, b3, c3) - x * det(a2, b2, c2, a3, b3, c3, 1, 1, 1)
            + y * det(a1, b1, c1, a3, b3, c3, 1, 1, 1)) / det(a1, b1, c1, a2, b2, c2, 1, 1, 1);
        }

        #endregion Heightmap - Private Static Members

        #region Heightmap - Public Static Members

        //PUBLIC METHODS

        /// <summary>
        /// Loads the specified heightmap from the a file.
        /// </summary>
        /// <param name="filename">filename to load</param>
        /// <param name="info">Heightmap info</param>
        /// <param name="heightmap">out heightmap</param>
        /// <returns>True/False heightmap can be loaded</returns>
        public static bool LoadFromFile(string filename, HeightMapInfo info, out HeightMap heightmap)
        {
            bool result = false;
            FileStream filestream = null;
            try
            {
                //OPEN THE FILESTREAM
                filestream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(filestream);

                //GENERATE NEW HEIGHTMAP
                HeightMap hmap = new HeightMap();
                hmap.info = info;

                //GENERATE MAX X, Y VALUES
                int MaxX = hmap.info.size;
                int MaxY = hmap.info.size;

                //GENERATE X/Y ARRAY FOR
                hmap.HeightData = new float[MaxX, MaxY];

                //READ ALL Z COORDS
                float size = (hmap.info.size / 2);
                for (int y = 0; y < MaxY; y++)
                {
                    for (int x = 0; x < MaxX; x++)
                    {
                        hmap.HeightData[x, y] = (float)(((float)reader.ReadUInt16() - 32768) / 32768) * size;
                    }
                }

                //CLOSE DOWN RESOURCES
                reader.Close();
                filestream.Close();
                heightmap = hmap;
                result = true;
            }
            catch (IOException)
            {
                //A IO-EXCEPTION OCCURED
                Trace.TraceError("Cannot load heightmap from file: {0}", filename);
                heightmap = null;
                result = false;
            }
            catch (OutOfMemoryException)
            {
                //WE RUN OUT OF MEMORY WHILE LOADING A MAP, THIS CAN BE THE RESULT OF CORRUPT MEMORY
                Trace.TraceError("Ran out of memory when attempting to load: {0}", filename);
                heightmap = null;
                result = false;
            }
            finally
            {
                //CLOSE DOWN RESOURCES
                if (filestream != null) filestream.Close();
            }

            return result;
        }

        #endregion Heightmap - Public Static Members

        #region Heightmap - Nested

        //NESTED CLASSES

        /// <summary>
        /// Internal 2D-Point structure used in the z-calculations.
        /// </summary>
        private struct point2D
        {
            public float x, y;
        }

        /// <summary>
        /// Internal 3D-Point structure used in the z-calcuations.
        /// </summary>
        private struct point3D
        {
            public float x, y, z;
        }

        /// <summary>
        /// Internal heightmap structure used with to speciafy the scale
        /// and x,y offsets for the associated heightmap.
        /// </summary>
        public class HeightMapInfo
        {
            /// <summary>
            /// Size of the heightmap
            /// </summary>
            public int size;

            /// <summary>
            /// Scale of the heightmap
            /// </summary>
            public int[] scale = new int[3];

            /// <summary>
            /// Starting location of the heightmap
            /// </summary>
            public float[] location = new float[3];
        }

        #endregion Heightmap - Nested

        #region Heightmap - Constructor/Deconstructor

        /// <summary>
        /// Creates a new heightmap
        /// </summary>
        public HeightMap()
        {
            this.HeightData = new float[0, 0];
            this.info = new HeightMapInfo();
        }

        #endregion Heightmap - Constructor/Deconstructor
    }
}