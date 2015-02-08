using Saga.Map.Definitions.Misc;
using System;
using System.Collections.Generic;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public class Addition : IEnumerable<AdditionState>
    {
        [NonSerialized()]
        protected internal volatile bool _skipcheck = false;

        public List<AdditionState> additions = new List<AdditionState>();
        public List<AdditionState> timed_additions = new List<AdditionState>();

        #region IEnumerable<AdditionState> Members

        public IEnumerator<AdditionState> GetEnumerator()
        {
            for (int i = 0; i < additions.Count; i++)
            {
                yield return additions[i];
            }
            for (int i = 0; i < timed_additions.Count; i++)
            {
                yield return timed_additions[i];
            }
        }

        #endregion IEnumerable<AdditionState> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}