using Saga.Map;
using Saga.Network.Packets;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common
{
    public static class Experience
    {
        [DebuggerNonUserCode()]
        [CLSCompliant(false)]
        public static void GiveWeaponExperience(Character target, uint wexp)
        {
            byte index = target.weapons.ActiveWeaponIndex == 1 ? target.weapons.SeconairyWeaponIndex : target.weapons.PrimaryWeaponIndex;
            if (index < target.weapons.UnlockedWeaponSlots)
            {
                Weapon weapon = target.weapons[index];
                if (weapon == null || weapon._weaponlevel >= Singleton.experience.MaxWLVL) return;

                //Checks if the weapon is  at already at max experience
                uint ReqWexp = Singleton.experience.FindRequiredWexp(weapon._weaponlevel + 1);
                if (weapon._experience == ReqWexp)
                {
                    return;
                }

                //Adds the experience and check if we gain a level
                weapon._experience += wexp;
                if (weapon._experience > ReqWexp)
                {
                    weapon._experience = ReqWexp;
                }

                //Adds the weapon experience
                SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
                spkt.Function = 2;
                spkt.Value = weapon._experience;
                spkt.Slot = (byte)index;
                spkt.SessionId = target.id;
                target.client.Send((byte[])spkt);
            }
        }

        [DebuggerNonUserCode()]
        [CLSCompliant(false)]
        public static void GiveSkillExperience(Character target, uint skillid, uint experiencepoints)
        {
            throw new NotImplementedException();
        }

        [DebuggerNonUserCode()]
        [CLSCompliant(false)]
        public static void GiveJobExperience(Character target, uint experiencepoints)
        {
            throw new NotImplementedException();
        }

        [DebuggerNonUserCode()]
        [CLSCompliant(false)]
        public static void GiveCharacterExperience(Character target, uint experiencepoints)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a set of exp to your character.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="cexp"></param>
        /// <param name="jexp"></param>
        public static void Add(Character character, uint cexp, uint jexp, uint wexp)
        {
            byte clvl_up = 0;
            byte jlvl_up = 0;
            int result = 0;

            GiveWeaponExperience(character, wexp);

            try
            {
                #region Check for character-level

                if (character._level + 1 <= Singleton.experience.MaxCLVL)
                {
                    uint req_cexp = Singleton.experience.FindRequiredCexp((byte)(character._level + 1));
                    if (cexp > 0)
                    {
                        result |= 32;
                        result |= (character.Cexp + cexp >= req_cexp) ? 16 : 0;
                    }
                }
                else
                {
                    cexp = 0;
                }

                #endregion Check for character-level

                #region Check for job-level

                if (character.jlvl + 1 <= Singleton.experience.MaxJLVL)
                {
                    uint req_jexp = Singleton.experience.FindRequiredJexp((byte)(character.jlvl + 1));
                    if (jexp > 0)
                    {
                        result |= 4;
                        result |= (character.Jexp + jexp >= req_jexp) ? 1 : 0;
                    }
                }
                else
                {
                    jexp = 0;
                }

                #endregion Check for job-level

                #region Quick Lock For updates

                lock (character)
                {
                    //CALCULATE EXPERIENCE
                    uint newCexp = character.Cexp + cexp;
                    uint newJexp = character.Jexp + jexp;

                    //COMPUTE THE LEVEL DIFFERENCE (FORWARD ONLY)
                    if ((result & 1) == 1) jlvl_up = Singleton.experience.FindJlvlDifference(character.jlvl, newJexp);
                    if ((result & 16) == 16) clvl_up = Singleton.experience.FindClvlDifference(character._level, newCexp);

                    //RECALCULATE THE SP / HP ON CLVL UP
                    if (clvl_up > 0)
                    {
                        //CALCULATE BASE MAX HP/SP
                        ushort _HPMAX = Singleton.CharacterConfiguration.CalculateMaximumHP(character);
                        ushort _SPMAX = Singleton.CharacterConfiguration.CalculateMaximumSP(character);

                        //SUBSTRACT FROM CURRENT MAX HP/SP
                        character._status.MaxHP -= _HPMAX;
                        character._status.MaxSP -= _SPMAX;

                        //ADD CLVL'S
                        character._level += clvl_up;

                        //CALCULATE NEW BASE MAX HP/SP
                        ushort _HPMAX2 = Singleton.CharacterConfiguration.CalculateMaximumHP(character);
                        ushort _SPMAX2 = Singleton.CharacterConfiguration.CalculateMaximumSP(character);

                        //GENERATE STATS
                        character._status.MaxHP += _HPMAX2;
                        character._status.MaxSP += _SPMAX2;
                        character.stats.REMAINING += (ushort)(clvl_up * 2);

                        //FINALIZE SEND UPDATE
                        CommonFunctions.SendExtStats(character);
                        CommonFunctions.SendBattleStatus(character);

                        if (character.sessionParty != null)
                        {
                            SMSG_PARTYMEMBERCLVL spkt = new SMSG_PARTYMEMBERCLVL();
                            spkt.Index = 1;
                            spkt.ActorId = character.id;
                            spkt.Lp = character.jlvl;
                            foreach (Character target in character.sessionParty.GetCharacters())
                            {
                                spkt.SessionId = target.id;
                                target.client.Send((byte[])spkt);
                            }
                        }
                    }

                    if (jlvl_up > 0)
                    {
                        character.jlvl += jlvl_up;
                        if (character.sessionParty != null)
                        {
                            SMSG_PARTYMEMBERJLVL spkt = new SMSG_PARTYMEMBERJLVL();
                            spkt.Index = 1;
                            spkt.ActorId = character.id;
                            spkt.Jvl = character.jlvl;
                            foreach (Character target in character.sessionParty.GetCharacters())
                            {
                                spkt.SessionId = target.id;
                                target.client.Send((byte[])spkt);
                            }
                        }
                    }

                    //SET THE EXP
                    character.Cexp = newCexp;
                    character.Jexp = newJexp;

                    if (clvl_up > 0 || jlvl_up > 0)
                    {
                        //RESET HP AND SP
                        character.HP = character.HPMAX;
                        character.SP = character.SPMAX;
                    }

                    //FINALIZE SEND UPDATE
                    CommonFunctions.UpdateCharacterInfo(character, (byte)result);
                }

                #endregion Quick Lock For updates

                #region Structurize Buffer

                List<RelayPacket> buffer = new List<RelayPacket>();
                if (clvl_up > 0)
                {
                    //LEVEL UP CHARACTER-BASE LEVEL
                    SMSG_LEVELUP spkt = new SMSG_LEVELUP();
                    spkt.ActorID = character.id;
                    spkt.Levels = clvl_up;
                    spkt.LevelType = 1;
                    buffer.Add(spkt);
                }
                if (jlvl_up > 0)
                {
                    //LEVEL UP CHARACTER-JOB LEVEL
                    SMSG_LEVELUP spkt = new SMSG_LEVELUP();
                    spkt.ActorID = character.id;
                    spkt.Levels = jlvl_up;
                    spkt.LevelType = 2;
                    buffer.Add(spkt);
                }

                #endregion Structurize Buffer

                #region Flush all updates

                foreach (MapObject c in character.currentzone.GetObjectsInRegionalRange(character))
                    if (MapObject.IsPlayer(c))
                    {
                        Character current = c as Character;
                        foreach (RelayPacket buffered_packet in buffer)
                        {
                            buffered_packet.SessionId = current.id;
                            current.client.Send((byte[])buffered_packet);
                        }
                    }

                #endregion Flush all updates
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }
    }
}