using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

namespace Saga.Structures
{
    public class PartySession : IEnumerable<Character>
    {
        public PartySession(Character character)
        {
            this._PartyLeader = character;
            this._Characters.Add(character);
        }

        internal List<Character> _Characters = new List<Character>();
        internal byte ExpSettings = 1;
        internal byte LootSettings = 1;

        internal Character _PartyLeader = null;
        internal Character _LootLeader = null;

        public Character PartyLeader
        {
            get
            {
                return _PartyLeader;
            }
        }

        public Character ItemLeader
        {
            get
            {
                return _LootLeader;
            }
        }

        public int Count
        {
            get
            {
                return _Characters.Count;
            }
        }

        public int IndexOf(Character character)
        {
            Predicate<Character> FindChars = delegate(Character c)
            {
                return character.id == c.id;
            };

            return this._Characters.FindIndex(FindChars);
        }

        public void Add(Character character)
        {
            this._Characters.Add(character);
        }

        public void Remove(Character character)
        {
            this._Characters.Remove(character);
        }

        public IEnumerable<Character> GetCharacters()
        {
            foreach (Character c in _Characters)
                yield return c;
        }

        public bool IsMemberOfParty(uint Id)
        {
            bool result = false;
            foreach (Character c in _Characters)
                result |= c.ModelId == Id;
            return result;
        }

        public Character GetMemberById(uint Id)
        {
            Character member = null;
            foreach (Character memberA in this._Characters)
            {
                if (memberA.id == Id)
                {
                    member = memberA;
                    break;
                }
            }
            return member;
        }

        public Character this[int Index]
        {
            get
            {
                return this._Characters[Index];
            }
        }

        #region IEnumerable<Character> Members

        public IEnumerator<Character> GetEnumerator()
        {
            foreach (Character c in _Characters)
                yield return c;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<Character> Members
    }
}