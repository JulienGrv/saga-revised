using Saga.Enumarations;
using Saga.PrimaryTypes;
using System.Collections.Generic;

namespace Saga.Tasks
{
    /// <summary>
    /// LifeCycle taks processes all regeneration attributes. That contributes to
    /// the users in game experience.
    /// </summary>
    public static class LifeCycle
    {
        #region Private Members

        /// <summary>
        /// List of users associated by their username
        /// </summary>
        private static Dictionary<string, Character> ListOfUsersByName = new Dictionary<string, Character>();

        /// <summary>
        /// List of users associated by their session id.
        /// </summary>
        private static Dictionary<uint, Character> ListOfUsers = new Dictionary<uint, Character>(30);

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Get's the number of users
        /// </summary>
        public static int Count
        {
            get
            {
                return ListOfUsersByName.Count;
            }
        }

        /// <summary>
        /// Returns an array of characters
        /// </summary>
        public static IEnumerable<Character> Characters
        {
            get
            {
                lock (ListOfUsersByName)
                {
                    foreach (KeyValuePair<string, Character> pair in ListOfUsersByName)
                        yield return pair.Value;
                }
            }
        }

        /// <summary>
        /// Get character by name
        /// </summary>
        /// <returns></returns>
        public static bool TryGetByName(string name, out Character character)
        {
            return ListOfUsersByName.TryGetValue(name.ToUpperInvariant(), out character);
        }

        /// <summary>
        /// Get character by id
        /// </summary>
        public static bool TryGetById(uint id, out Character character)
        {
            return ListOfUsers.TryGetValue(id, out character);
        }

        /// <summary>
        /// This functions subscribes a character to a list of users
        /// indexed by their sessions id. (Is the same as the character
        /// id.
        /// </summary>
        /// <param name="character"></param>
        public static void Subscribe(Character character)
        {
            if (character == null) return;
            lock (ListOfUsers)
            {
                if (ListOfUsers.ContainsKey(character.id) == false)
                {
                    lock (ListOfUsersByName)
                    {
                        ListOfUsersByName.Add(character.Name.ToUpperInvariant(), character);
                    }

                    ListOfUsers.Add(character.id, character);
                }
            }
        }

        /// <summary>
        /// This function unsubscribes a character from the list of users
        /// indexed by their session id. (Is the same as the character id.
        /// </summary>
        /// <param name="character"></param>
        public static void Unsubscribe(Character character)
        {
            if (character == null) return;
            lock (ListOfUsersByName)
            {
                ListOfUsersByName.Remove(character.Name.ToUpperInvariant());
            }
            lock (ListOfUsers)
            {
                ListOfUsers.Remove(character.id);
            }
        }

        #endregion Public Members

        #region Intenernal Members

        /// <summary>
        /// Process all characters in the desginated list for regeneration
        /// etc.
        /// </summary>
        /// <remarks>
        /// This function proccessed all characters in the specified for the list.
        /// regeneration of both HP and SP. We need to process them both here to
        /// prevent duplicate packets.
        ///
        /// For example when we would have used a timer apprpoach here we'ld have to set
        /// two sepperate timers for both SP and HP. Both can have their own recovery rates.
        ///
        /// Note: When a player's state is seen as dead, no packets are processed. So only
        /// process thsses packets when players stance ! 7 (where 7 resembles death)
        ///
        /// To prevent duplicate packets we'll set UPDATE to true once 1 on the functions, read
        /// SPRECOVERY or HPRECOVERY is processed in the same scope. Once we're finished with
        /// processing send the character updates.
        /// </remarks>
        internal static void Process()
        {
            foreach (KeyValuePair<uint, Character> c in ListOfUsers)
            {
                //HELPER VARIABLES
                bool result2 = false;
                int result = c.Value.UpdateReason;

                //PROCESS REGENERATIONS
                if (c.Value.client.isloaded == true)
                {
                    if (c.Value.stance != 7)
                    {
                        result2 = c.Value.OnRegenerateHP() | c.Value.OnRegenerateSP() | c.Value.OnBreath(); //| c.Value.OnLPReduction();
                    }

                    //PROCESS UPDATES
                    if (result > 0 || result2 == true)
                        Update(c.Value);
                    c.Value.UpdateAdditions();
                }
            }
        }

        public static void Update(Character character)
        {
            unchecked
            {
                if ((character._status.Updates & 1) == 1)
                {
                    CommonFunctions.UpdateCharacterInfo(character, (byte)character.UpdateReason);
                    if (character.HP == 0)
                    {
                        character.stance = (byte)StancePosition.Dead;
                        character.OnDie();
                        CommonFunctions.UpdateState(character);
                        character._status.Updates &= (byte)(~1);
                    }

                    if (character.sessionParty != null)
                    {
                        foreach (Character target in character.sessionParty._Characters)
                        {
                            if (target.id == character.id) continue;
                            Common.Actions.UpdateMemberHp(target, character);
                            Common.Actions.UpdateMemberSp(target, character);
                        }
                    }
                }
                if ((character._status.Updates & 2) == 2)
                {
                    CommonFunctions.SendBattleStatus(character);
                    character._status.Updates &= (byte)(~2);
                }
                if ((character._status.Updates & 3) == 3)
                {
                    CommonFunctions.SendExtStats(character);
                    character._status.Updates &= (byte)(~3);
                }
            }
        }

        #endregion Intenernal Members
    }
}