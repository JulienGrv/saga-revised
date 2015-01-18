using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Map.Utils.Definitions.Misc
{

    [Serializable()]
    public class Skill
    {
        public Skill() { }
        public uint Id;
        public uint Experience;
        public Saga.Factory.Spells.Info info;
    }
}
