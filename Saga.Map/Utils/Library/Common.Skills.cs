using Saga;
using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Common
{
    public static class Skills
    {
        #region Private Methods

        private static bool HasSpecialRootSkillPresent(Character target, uint SkillId)
        {
            for (int i = 0; i < 16; i++)
            {
                Skill skill = target.SpecialSkills[i];
                if (skill != null && (skill.Id - (skill.Id % 100)) == SkillId) return true;
            }
            return false;
        }

        #endregion Private Methods

        #region Internal Methods

        /// <summary>
        /// Adds a additions icon the the player.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addition"></param>
        /// <param name="time"></param>
        internal static void SendExchangeStatus(Actor source, uint addition, uint time)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(SearchFlags.Characters))
            {
                SMSG_ADDITIONBEGIN spkt = new SMSG_ADDITIONBEGIN();
                spkt.Duration = time;
                spkt.SourceActor = source.id;
                spkt.StatusID = addition;
                spkt.SessionId = current.id;
                current.client.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Deletes a given addition icon to a player
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addition"></param>
        internal static void SendDeleteStatus(Actor source, uint addition)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(SearchFlags.Characters))
            {
                SMSG_ADDITIONEND spkt = new SMSG_ADDITIONEND();
                spkt.SourceActor = source.id;
                spkt.StatusID = addition;
                spkt.SessionId = current.id;
                current.client.Send((byte[])spkt);
            }
        }

        #endregion Internal Methods

        #region Public Methods

        /// <summary>
        /// Use a offensive skills
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="skillid"></param>
        /// <param name="skilltype"></param>
        /// <remarks>
        /// This functions is invoked by mob ai. The character has it's own skill handler
        /// inside the network client.
        /// </remarks>
        public static void OffensiveSkillUse(MapObject target, MapObject source, uint skillid, byte skilltype)
        {
            try
            {
                SkillUsageEventArgs argument = null;
                if (SkillUsageEventArgs.Create(skillid, source, target, out argument) && argument.Use())
                {
                    Predicate<Character> SendToCharacter = delegate(Character forwardTarget)
                    {
                        //Skill sucess
                        SMSG_OFFENSIVESKILL spkt = new SMSG_OFFENSIVESKILL();
                        spkt.SkillID = skillid;
                        spkt.SkillType = 1;
                        spkt.TargetActor = target.id;
                        spkt.SourceActor = source.id;
                        spkt.IsCritical = (forwardTarget.id == source.id || forwardTarget.id == target.id) ? (byte)argument.Result : (byte)7;
                        spkt.Damage = argument.Damage;
                        spkt.SessionId = forwardTarget.id;
                        forwardTarget.client.Send((byte[])spkt);

                        //Process some general updates
                        if (forwardTarget._targetid == target.id)
                            Common.Actions.SelectActor(forwardTarget, target as Actor);
                        if (argument.TargetHasDied)
                            Common.Actions.UpdateStance(forwardTarget, target as Actor);
                        if (argument.TargetHasDied && MapObject.IsNpc(target))
                            Common.Actions.UpdateIcon(forwardTarget, target as BaseMob);
                        if (forwardTarget.id == target.id)
                            LifeCycle.Update(forwardTarget);

                        return true;
                    };

                    Regiontree tree = source.currentzone.Regiontree;
                    foreach (Character forwardTarget in tree.SearchActors(source, SearchFlags.Characters))
                    {
                        SendToCharacter(forwardTarget);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static bool HasRootSkillPresent(Character target, uint skill)
        {
            //Helper variables
            uint nSkillId = (skill - (skill % 100));
            Predicate<Skill> FindSkill = delegate(Skill match)
            {
                return (match.Id - (match.Id % 100)) == nSkillId;
            };

            //Result
            bool result = target.learnedskills.FindIndex(
                new Predicate<Skill>(FindSkill)
            ) > -1;

            //Check wether to check the specialskills
            if (result == true)
                return true;
            else
                return HasSpecialRootSkillPresent(target, nSkillId);
        }

        public static bool HasSpecialSkillPresent(Character target, uint skill)
        {
            for (int i = 0; i < 16; i++)
            {
                Skill specialskill = target.SpecialSkills[i];
                if (specialskill != null && specialskill.Id == skill) return true;
            }
            return false;
        }

        /// <summary>
        /// Sends a skill effect to the selected players.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addition"></param>
        /// <param name="effect"></param>
        /// <param name="amount"></param>
        public static void SendSkillEffect(Actor source, uint addition, byte effect, uint amount)
        {
            if (source == null)
                return;

            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character target in tree.SearchActors(source, SearchFlags.Characters))
            {
                SMSG_SKILLEFFECT spkt = new SMSG_SKILLEFFECT();
                spkt.SourceActor = source.id;
                spkt.Unknown1 = 1;
                spkt.Unknown2 = addition;
                spkt.Amount = amount;
                spkt.Function = effect;
                spkt.SessionId = target.id;
                target.client.Send((byte[])spkt);
            }
        }

        public static void SendSkillEffect(Character target, Actor source, uint addition, byte effect, uint amount)
        {
            SMSG_SKILLEFFECT spkt = new SMSG_SKILLEFFECT();
            spkt.SourceActor = source.id;
            spkt.Unknown1 = 1;
            spkt.Unknown2 = addition;
            spkt.Amount = amount;
            spkt.Function = effect;
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }

        public static bool HasAddition(Actor target, uint addition)
        {
            Predicate<AdditionState> c = delegate(AdditionState state)
            {
                return state.Addition == addition;
            };

            target._additions._skipcheck = true;
            bool result = target._additions.additions.Exists(c);
            target._additions._skipcheck = true;
            return result;
        }

        public static void DoAddition(Actor self, object target, uint addition)
        {
            Saga.Factory.Additions.Info info;
            if (Singleton.Additions.TryGetAddition(addition, out info))
            {
                info.Do(self, target, AdditionContext.Applied);
            }
        }

        /// <summary>
        /// Creates a new addition which is a static addition
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="Addition">Addition which is casted</param>
        public static void CreateAddition(Actor target, uint addition)
        {
            Saga.Factory.Additions.Info info;
            if (Singleton.Additions.TryGetAddition(addition, out info))
            {
                target._additions._skipcheck = true;
                Singleton.Additions.ApplyAddition(addition, target);
                AdditionState state = new AdditionState(addition, 0, info);
                state.canexpire = false;
                target._additions.additions.Add(state);
                SendExchangeStatus(target, addition, 0);
                target._additions._skipcheck = false;
            }
        }

        /// <summary>
        /// Creates a new addition which is a timed addition
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="addition">Addition which is casted</param>
        /// <param name="duration">How long the addition lasts (ms)</param>
        public static void CreateAddition(Actor target, uint addition, uint duration)
        {
            Saga.Factory.Additions.Info info;
            if (Singleton.Additions.TryGetAddition(addition, out info))
            {
                target._additions._skipcheck = true;
                Singleton.Additions.ApplyAddition(addition, target);
                AdditionState state = new AdditionState(addition, duration, info);
                state.canexpire = true;
                target._additions.timed_additions.Add(state);
                SendExchangeStatus(target, addition, duration);
                target._additions._skipcheck = false;
            }
        }

        /// <summary>
        /// Deletes a staitc addition
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="Addition">Addition which is casted</param>
        public static void DeleteAddition(Actor target, uint addition)
        {
            Predicate<AdditionState> c = delegate(AdditionState state)
            {
                return state.Addition == addition;
            };

            target._additions._skipcheck = true;
            int index = target._additions.timed_additions.FindIndex(c);
            if (index > -1) //reset lifetime
            {
                Singleton.Additions.DeapplyAddition(addition, target);
                target._additions.timed_additions.RemoveAt(index);
                SendDeleteStatus(target, addition);
            }
            target._additions._skipcheck = false;
        }

        /// <summary>
        /// Deletes a staitc addition
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="Addition">Addition which is casted</param>
        public static void DeleteStaticAddition(Actor target, uint addition)
        {
            Predicate<AdditionState> c = delegate(AdditionState state)
            {
                return state.Addition == addition;
            };

            target._additions._skipcheck = true;
            int index = target._additions.additions.FindIndex(c);
            if (index > -1) //reset lifetime
            {
                Singleton.Additions.DeapplyAddition(addition, target);
                target._additions.additions.RemoveAt(index);
                SendDeleteStatus(target, addition);
            }
            target._additions._skipcheck = false;
        }

        /// <summary>
        ///Updates a existing addition or removes the old addition to replace it
        ///by the new one (timed).
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="Addition">Addition which is casted</param>
        /// <param name="Duration">How long the addition lasts (ms)</param>
        public static void UpdateAddition(Actor target, uint addition, uint duration)
        {
            Saga.Factory.Additions.Info info;
            if (Singleton.Additions.TryGetAddition(addition, out info))
            {
                Predicate<AdditionState> c = delegate(AdditionState state)
                {
                    return state.Addition == addition;
                };

                target._additions._skipcheck = true;
                int index = target._additions.timed_additions.FindIndex(c);
                if (index > -1) //reset lifetime
                {
                    AdditionState state = target._additions.timed_additions[index];
                    state.Lifetime = duration;
                    SendDeleteStatus(target, addition);
                    SendExchangeStatus(target, addition, duration);
                }
                else //add new
                {
                    Singleton.Additions.ApplyAddition(addition, target);
                    AdditionState state = new AdditionState(addition, duration, info);
                    state.canexpire = true;
                    target._additions.timed_additions.Add(state);
                    SendExchangeStatus(target, addition, duration);
                }
                target._additions._skipcheck = false;
            }
        }

        /// <summary>
        ///Updates a existing addition or removes the old addition to replace it
        ///by the new one (static).
        /// </summary>
        /// <param name="target">Target on which the addition is casted</param>
        /// <param name="Addition">Addition which is casted</param>
        /// <param name="Duration">How long the addition lasts (ms)</param>
        public static void UpdateAddition(Actor target, uint addition)
        {
            Saga.Factory.Additions.Info info;
            if (Singleton.Additions.TryGetAddition(addition, out info))
            {
                Predicate<AdditionState> c = delegate(AdditionState state)
                {
                    return state.Addition == addition;
                };

                target._additions._skipcheck = true;
                int index = target._additions.additions.FindIndex(c);
                if (index > -1) //reset lifetime
                {
                    AdditionState state = target._additions.timed_additions[index];
                    state.Lifetime = info.EffectDuration;
                    SendDeleteStatus(target, addition);
                    SendExchangeStatus(target, addition, info.EffectDuration);
                }
                else //add new
                {
                    Singleton.Additions.ApplyAddition(addition, target);
                    AdditionState state = new AdditionState(addition, info.EffectDuration, info);
                    state.canexpire = true;
                    target._additions.timed_additions.Add(state);
                    SendExchangeStatus(target, addition, info.EffectDuration);
                }
                //if (index < 0) //reset lifetime
                //{
                //    Singleton.Additions.ApplyAddition(addition, target);
                //    AdditionState state = new AdditionState(addition, 0, info);
                //    state.canexpire = false;
                //    target._additions.additions.Add(state);
                //    SendExchangeStatus(target, addition, 0);
                //}
                target._additions._skipcheck = false;
            }
        }

        #endregion Public Methods
    }
}