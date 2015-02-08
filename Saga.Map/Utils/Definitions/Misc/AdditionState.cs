using Saga.Enumarations;
using Saga.Factory;
using System;
using System.Globalization;

namespace Saga.Map.Definitions.Misc
{
    [Serializable()]
    public class AdditionState
    {
        public uint Addition;
        public bool canexpire;
        private int LastUpdate;
        private int LastIntervalTick;

        public int Duration;
        internal Additions.Info info;

        internal AdditionState(uint addition, uint MaxDuration, Additions.Info info)
        {
            this.Addition = addition;
            Duration = (int)MaxDuration;
            LastUpdate = Environment.TickCount;
            LastIntervalTick = Environment.TickCount;
            this.info = info;
        }

        public uint Lifetime
        {
            get
            {
                return (uint)Duration;
            }
            set
            {
                LastUpdate = Environment.TickCount;
                LastIntervalTick = Environment.TickCount;
                Duration = (int)value;
            }
        }

        public bool CanExpire
        {
            get
            {
                return this.canexpire;
            }
        }

        public bool IsExpired
        {
            get
            {
                return Duration <= 0;
            }
        }

        internal void Update(object sender, object target)
        {
            int tick = Environment.TickCount - LastUpdate;
            int tick2 = Environment.TickCount - LastIntervalTick;

            if (tick > 1000)
            {
                Duration -= tick;
                LastUpdate = Environment.TickCount;
            }

            if (info.Interval > 0 && tick2 > info.Interval)
            {
                info.Do(sender, target, AdditionContext.Reapplied);
                LastIntervalTick = Environment.TickCount;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is AdditionState)
            {
                AdditionState obj2 = obj as AdditionState;
                return obj2.Addition == this.Addition;
            }
            else if (obj is uint)
            {
                return this.Addition == Convert.ToUInt32(obj, NumberFormatInfo.InvariantInfo);
            } return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}