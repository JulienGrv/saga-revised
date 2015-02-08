using Saga.Map;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

namespace Saga.Structures
{
    public class HateCollection
    {
        /// <summary>
        /// Private Hateable
        /// </summary>
        private Dictionary<uint, short> HateTable = new Dictionary<uint, short>();

        /// <summary>
        /// Adds or substracts an amount of hate for the specified object
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="hate"></param>
        public void Add(MapObject objects, short hate)
        {
            short previoushate = 0;
            HateTable.TryGetValue(objects.id, out previoushate);
            previoushate = previoushate + hate > short.MaxValue ? (short)short.MaxValue : (short)(previoushate + hate);

            lock (HateTable)
            {
                if (previoushate == 0)
                {
                    HateTable.Remove(objects.id);
                }
                else
                {
                    HateTable[objects.id] = previoushate;
                }
            }
        }

        /// <summary>
        /// Clears all hate for the specified object
        /// </summary>
        /// <param name="objects"></param>
        public void Clear(MapObject objects)
        {
            lock (HateTable)
            {
                HateTable.Remove(objects.id);
            }
        }

        /// <summary>
        /// Get's or the amount of hate for the specified object
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public short this[MapObject objects]
        {
            get
            {
                short previoushate = 0;
                HateTable.TryGetValue(objects.id, out previoushate);
                return previoushate;
            }
        }
    }

    public class DamageCollection
    {
        /// <summary>
        /// Private Hateable
        /// </summary>
        private Dictionary<uint, uint> DamageTable = new Dictionary<uint, uint>();

        /// <summary>
        /// Adds or substracts an amount of hate for the specified object
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="hate"></param>
        public void Add(MapObject objects, uint damage)
        {
            if (damage > 0)
            {
                uint previousdamage = 0;
                DamageTable.TryGetValue(objects.id, out previousdamage);
                previousdamage = previousdamage + damage > uint.MaxValue ? (uint)uint.MaxValue : (uint)(previousdamage + damage);

                lock (DamageTable)
                {
                    if (previousdamage == 0)
                    {
                        DamageTable.Remove(objects.id);
                    }
                    else
                    {
                        DamageTable[objects.id] = previousdamage;
                    }
                }
            }
        }

        /// <summary>
        /// Get's or the amount of hate for the specified object
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public uint this[MapObject objects]
        {
            get
            {
                uint previousdamage = 0;
                DamageTable.TryGetValue(objects.id, out previousdamage);
                return previousdamage;
            }
        }

        /// <summary>
        /// Gives the experience rewards based on a damage history.
        /// </summary>
        /// <param name="killer">Character who killed the actor</param>
        /// <param name="mob">Mob that was died</param>
        /// <param name="maxcexp">Maximum character experience to handout</param>
        /// <param name="maxjexp">Maximum job experience to handout</param>
        /// <param name="maxwexp">Maximum weapon experience to handout</param>
        public void GiveExperienceRewards(Character killer, Monster mob, uint maxcexp, uint maxjexp, uint maxwexp)
        {
            lock (DamageTable)
            {
                uint HateTowardsOwner = 0;
                List<KeyValuePair<uint, uint>> PartyInstances = new List<KeyValuePair<uint, uint>>();
                List<KeyValuePair<uint, uint>> OtherInstances = new List<KeyValuePair<uint, uint>>();
                foreach (KeyValuePair<uint, uint> pair in this.DamageTable)
                {
                    //Just a helper object
                    MapObject temp;

                    //If hate is formysel or one of my party members stack me.
                    if (killer.id == pair.Key || (killer.sessionParty != null && killer.sessionParty.IsMemberOfParty(killer.id)))
                    {
                        PartyInstances.Add(pair);
                    }
                    //If i'm not a party member, but am visible range.
                    else if (Regiontree.TryFind(pair.Key, mob, out temp)
                          && killer.currentzone.IsInSightRangeByRadius(temp.Position, killer.Position))
                    {
                        OtherInstances.Add(pair);
                    }
                    //Give all other hate towards owner.
                    else
                    {
                        HateTowardsOwner += pair.Value;
                    }
                }

                //Handout experience to other people
                foreach (KeyValuePair<uint, uint> pair in OtherInstances)
                {
                    //Helper object
                    MapObject temp;
                    if (Regiontree.TryFind(pair.Key, mob, out temp))
                    {
                        //Get character
                        Character currentTarget = temp as Character;

                        //Scalar functions
                        double Percentage = (double)(15 + (5 - Math.Max(0, Math.Min(5, mob._level - killer._level))) * 17) / (double)100;
                        double CexpScalar = 1 + ((double)currentTarget._CexpModifier / (double)20000);
                        double JexpScalar = 1 + ((double)currentTarget._JexpModifier / (double)20000);
                        double DamageScalar = (double)pair.Value / (double)mob.HPMAX;

                        //Add the experience
                        if (currentTarget.client.isloaded == true && currentTarget.stance != 7)
                            Common.Experience.Add(
                                currentTarget,
                                (uint)(((double)maxcexp * Singleton.experience.Modifier_Cexp * DamageScalar) * Percentage * CexpScalar),
                                (uint)(((double)maxjexp * Singleton.experience.Modifier_Jexp * DamageScalar) * Percentage * JexpScalar),
                                1
                            );
                    }
                }

                //Handout experience to party people
                bool DivideByClvl = false;
                double AverageClvl = 0;
                double PartyScalar = 1;

                if (killer.sessionParty != null)
                {
                    DivideByClvl = killer.sessionParty.ExpSettings == 1;
                    PartyScalar = (double)(((float)1 + ((float)killer.sessionParty.Count * 0.2)) / (float)killer.sessionParty.Count);
                }

                if (DivideByClvl == true)
                {
                    if (killer.sessionParty != null)
                        foreach (Character currentTarget in killer.sessionParty)
                        {
                            AverageClvl += currentTarget._level;
                        }

                    if (killer.sessionParty != null)
                        foreach (Character currentTarget in killer.sessionParty)
                        {
                            if (!(currentTarget.map == killer.map && Point.IsInSightRangeByRadius(currentTarget.Position, killer.Position))) continue;
                            //Scalar functions
                            double Percentage = (double)(15 + (5 - Math.Max(0, Math.Min(5, mob._level - currentTarget._level))) * 17) / (double)100;
                            double Scalar = (double)1 - (double)((double)currentTarget._level / (double)AverageClvl);

                            //Add the experience
                            if (currentTarget.client.isloaded == true && currentTarget.stance != 7)
                                Common.Experience.Add(
                                    currentTarget,
                                    (uint)(((double)maxcexp * Singleton.experience.Modifier_Cexp * Scalar) * PartyScalar * Percentage * currentTarget._CexpModifier),
                                    (uint)(((double)maxjexp * Singleton.experience.Modifier_Jexp * Scalar) * PartyScalar * Percentage * currentTarget._JexpModifier),
                                    (uint)(currentTarget.ModelId == killer.ModelId ? ((double)maxwexp * currentTarget._WexpModifier) : 0)
                                );
                        }
                }
                else
                {
                    foreach (KeyValuePair<uint, uint> pair in PartyInstances)
                    {
                        //Helper object
                        MapObject temp;
                        if (!Regiontree.TryFind(pair.Key, mob, out temp)) continue;

                        //Get character
                        Character currentTarget = temp as Character;
                        if (currentTarget == null) continue;

                        //Scalar functions
                        double Percentage = (double)(15 + (5 - Math.Max(0, Math.Min(5, mob._level - currentTarget._level))) * 17) / (double)100;
                        double WexpScalar = 1 + ((double)currentTarget._WexpModifier / (double)20000);
                        double Scalar = 1;
                        if (currentTarget.id == killer.id)
                        {
                            Scalar = ((double)pair.Value + (double)HateTowardsOwner) / (double)mob.HPMAX;
                        }
                        else
                        {
                            Scalar = (double)pair.Value / (double)mob.HPMAX;
                        }

                        //Add the experience
                        if (currentTarget.client.isloaded == true && currentTarget.stance != 7)
                            Common.Experience.Add(
                                currentTarget,
                                (uint)(((double)maxcexp * (Scalar + Singleton.experience.Modifier_Cexp)) * PartyScalar * Percentage * currentTarget._CexpModifier),
                                (uint)(((double)maxjexp * (Scalar + Singleton.experience.Modifier_Jexp)) * PartyScalar * Percentage * currentTarget._JexpModifier),
                                (uint)(currentTarget.ModelId == killer.ModelId ? ((double)maxwexp * currentTarget._WexpModifier) : 0)
                            );
                    }
                }

                DamageTable.Clear();
            }
        }
    }

    public class CoolDownCollection
    {
        private Dictionary<uint, int> cooldowntable = new Dictionary<uint, int>();

        public bool IsCoolDown(uint skill)
        {
            uint rootskill = (skill / 100) * 100;

            int tick;
            if (cooldowntable.TryGetValue(rootskill, out tick))
                return (tick - Environment.TickCount) > 0;
            else
                return false;
        }

        public void Add(uint skill, int cooldown)
        {
            uint rootskill = (skill / 100) * 100;
            int nexttick = Environment.TickCount + cooldown;
            cooldowntable[rootskill] = nexttick;
        }

        public void Update()
        {
            List<uint> removestack = new List<uint>();
            foreach (KeyValuePair<uint, int> pair in cooldowntable)
            {
                if (pair.Value <= Environment.TickCount)
                {
                    removestack.Add(pair.Key);
                }
            }

            foreach (uint pair in removestack)
            {
                cooldowntable.Remove(pair);
            }
        }
    }

    public interface IHateable : IActorid
    {
        HateCollection Hatetable { get; }
    }

    public interface IActorid
    {
        uint Id { get; }
    }
}