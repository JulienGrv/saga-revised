using Saga.Structures;
using System.Collections.Generic;

namespace Saga.Paths
{
    public class WaypointedPath
    {
        /// <summary>
        /// Returns the current step
        /// </summary>
        private int currentstep = 0;

        /// <summary>
        /// Returns a list of waypoints
        /// </summary>
        private List<Point> waypointlist = new List<Point>();

        /// <summary>
        /// Returns the current step
        /// </summary>
        public int Step
        {
            get
            {
                return currentstep;
            }
        }

        /// <summary>
        /// Checks if the current node is at it's end.
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return currentstep < waypointlist.Count;
            }
        }

        /// <summary>
        /// Returns the next waypoint
        /// </summary>
        /// <returns></returns>
        public Point Next()
        {
            int a = currentstep;
            currentstep = ++currentstep % waypointlist.Count;
            return waypointlist[a];
        }

        /// <summary>
        /// Peeks the next waypoint
        /// </summary>
        /// <returns></returns>
        public Point Peek()
        {
            int a = currentstep + 1;
            a = a % waypointlist.Count;
            return waypointlist[a];
        }
    }
}