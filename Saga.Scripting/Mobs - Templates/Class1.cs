using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Templates;
using System;

namespace Saga.Scripting
{
    internal class GoldenThieftBug : RandomizedMonster, IArtificialIntelligence
    {
        #region Time difference

        /// <summary>
        /// Gets the last updated tick
        /// </summary>
        /// <returns>Time diffence between now and the last update</returns>
        private int GetUpdateTick()
        {
            return Environment.TickCount - Lifespan.lasttick;
        }

        private void UpdateTick()
        {
            Lifespan.lasttick = Environment.TickCount;
        }

        #endregion Time difference

        #region Mathematics

        private int YawOf(Point A, Point B)
        {
            return (int)(Math.Atan2(B.y - A.y, B.x - A.x) * 32768 / Math.PI);
        }

        private double DistanceOf(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return Math.Abs(distance);
        }

        #endregion Mathematics

        #region OOP-AI

        private void UpdatePosition(int t_diff)
        {
            float velocity = (float)((int)this._status.WalkingSpeed / 1000);
            float diff = t_diff * velocity;
            Point Loc = this.Position;
            Loc.x += (float)(diff * Math.Cos(Yaw.rotation * (Math.PI / 32768)));
            Loc.y += (float)(diff * Math.Sin(Yaw.rotation * (Math.PI / 32768)));
            this.Position = Loc;
        }

        void IArtificialIntelligence.Process()
        {
            int t_diff = GetUpdateTick();
            if (t_diff > 8000)
            {
                //System.Console.WriteLine("Update mob");
                if (IsMoving == true && this._status.CannotMove > 0)
                {
                    UpdatePosition(t_diff);
                }

                if (this._target != null)
                {
                    StartMovement(this._target.Position);
                }

                UpdateTick();
            }
        }

        #endregion OOP-AI
    }
}