using Saga.Enumarations;
using Saga.Map.Librairies;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Occurs when casting a skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLCAST(CMSG_SKILLCAST cpkt)
        {
            lock (this.character.cooldowncollection)
            {
                try
                {
                    MapObject target;
                    uint skill = cpkt.SkillID;
                    byte skilltype = cpkt.SkillType;
                    Factory.Spells.Info info = null;
                    bool cancast = Regiontree.TryFind(cpkt.TargetActor, this.character, out target)
                              && Singleton.SpellManager.TryGetSpell(skill, out info)
                              && this.character._lastcastedskill == 0
                              && ((long)((uint)Environment.TickCount) - this.character._lastcastedtick) > 0
                              && (info.delay == 0 || !this.character.cooldowncollection.IsCoolDown(skill))
                              && (info.maximumrange == 0 || info.IsInRangeOf((int)(Vector.GetDistance2D(this.character.Position, target.Position))))
                              && info.requiredWeapons[this.character.weapons.GetCurrentWeaponType()] == 1
                              && this.character.jlvl >= info.requiredJobs[this.character.job - 1]
                              && this.character.Status.CurrentLp >= (info.requiredlp == 6 ? 1 : info.requiredlp)
                              && info.IsTarget(this.character, target);

                    if (cancast)
                    {
                        //Set anti-hack variables
                        this.character._lastcastedskill = skill;
                        this.character._lastcastedtick = (Environment.TickCount + (int)info.casttime);
                        this.character.cooldowncollection.Update();

                        //Notify all actors that cast is in progress
                        Regiontree tree = this.character.currentzone.Regiontree;
                        foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
                        {
                            if (!Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position) || regionObject.client.isloaded == false) continue;
                            SMSG_SKILLCAST spkt = new SMSG_SKILLCAST();
                            spkt.SourceActor = this.character.id;
                            spkt.TargetActor = target.id;
                            spkt.SkillID = skill;
                            spkt.SkillType = skilltype;
                            spkt.SessionId = regionObject.id;
                            regionObject.client.Send((byte[])spkt);
                        }
                    }
                    else
                    {
                        /*SMSG_SKILLCASTCANCEL spkt = new SMSG_SKILLCASTCANCEL();
                        spkt.SkillID = cpkt.SkillID;
                        spkt.SourceActor = this.character.id;
                        spkt.SkillType = cpkt.SkillType;
                        this.Send((byte[])spkt);*/

                        //Skill failed
                        SMSG_OFFENSIVESKILLFAILED spkt = new SMSG_OFFENSIVESKILLFAILED();
                        spkt.SkillID = cpkt.SkillID;
                        spkt.SkillType = cpkt.SkillType;
                        spkt.SourceActor = this.character.id;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError("Exception processing the skill cast");
                }
            }
        }

        /// <summary>
        /// Occurs when canceling a casted skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLCASTCANCEL(CMSG_SKILLCASTCANCEL cpkt)
        {
            lock (this.character.cooldowncollection)
            {
                try
                {
                    uint skill = cpkt.SkillID;
                    byte skilltype = cpkt.SkillType;
                    Factory.Spells.Info info = null;

                    if (Singleton.SpellManager.TryGetSpell(skill, out info))
                    {
                        this.character._lastcastedskill = 0;
                        this.character._lastcastedtick = Environment.TickCount;
                        this.character.cooldowncollection.Add(skill, (int)info.delay);
                        this.character.cooldowncollection.Update();
                    }

                    //Notify all actors that cast is in progress
                    Regiontree tree = this.character.currentzone.Regiontree;
                    foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
                    {
                        if (!Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position) || regionObject.client.isloaded == false) continue;
                        SMSG_SKILLCASTCANCEL spkt = new SMSG_SKILLCASTCANCEL();
                        spkt.SkillID = skill;
                        spkt.SourceActor = this.character.id;
                        spkt.SkillType = skilltype;
                        spkt.SessionId = this.character.id;
                        regionObject.client.Send((byte[])spkt);
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError("Exception processing the skill cancel");
                }
            }
        }

        /// <summary>
        /// Occurs when using an offensive skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_USEOFFENSIVESKILL(CMSG_OFFENSIVESKILL cpkt)
        {
            lock (this.character.cooldowncollection)
            {
                try
                {
                    MapObject target;
                    uint skillid = cpkt.SkillID;
                    byte skilltype = cpkt.SkillType;
                    SkillUsageEventArgs argument = null;

                    bool cancast = Regiontree.TryFind(cpkt.TargetActor, this.character, out target)
                              && SkillUsageEventArgs.Create(skillid, this.character, target, out argument)
                              && argument.SpellInfo.casttime > -1 && (argument.SpellInfo.casttime == 0 || this.character._lastcastedskill == skillid)
                              && ((long)((uint)Environment.TickCount) - this.character._lastcastedtick) > 0
                              && (argument.SpellInfo.delay == 0 || !this.character.cooldowncollection.IsCoolDown(skillid))
                              && (argument.SpellInfo.maximumrange == 0 || argument.SpellInfo.IsInRangeOf((int)(Vector.GetDistance2D(this.character.Position, target.Position))))
                              && argument.SpellInfo.requiredWeapons[this.character.weapons.GetCurrentWeaponType()] == 1
                              && this.character.jlvl >= argument.SpellInfo.requiredJobs[this.character.job - 1]
                              && this.character.Status.CurrentLp >= (argument.SpellInfo.requiredlp == 6 ? 1 : argument.SpellInfo.requiredlp)
                              && argument.SpellInfo.IsTarget(this.character, target);

                    if (cancast && argument.Use())
                    {
                        //Clear casted skill
                        this.character._lastcastedskill = 0;
                        this.character._lastcastedtick = Environment.TickCount;

                        //Set cooldown timer
                        int delay = (int)(argument.SpellInfo.delay - ((character.stats.Dexterity * 2) + (character.stats.Concentration * 2)));
                        if (delay > 0) this.character.cooldowncollection.Add(skillid, delay);
                        this.character.cooldowncollection.Update();

                        //Use required sp points
                        if (argument.SpellInfo.SP > -1)
                        {
                            this.character.LASTSP_TICK = Environment.TickCount;
                            this.character.SP = (ushort)(this.character.SP - argument.SpellInfo.SP);
                        }
                        //Set sp to 0
                        else if (argument.SpellInfo.SP == -1)
                        {
                            this.character.LASTSP_TICK = Environment.TickCount;
                            this.character.SP = 0;
                        }

                        //Use required lp points
                        if (argument.SpellInfo.requiredlp == 6)
                        {
                            this.character._status.CurrentLp = 0;
                            this.character._status.Updates |= 1;
                        }
                        else if (argument.SpellInfo.requiredlp > 0)
                        {
                            this.character._status.CurrentLp -= argument.SpellInfo.requiredlp;
                            this.character._status.Updates |= 1;
                        }

                        //Add intergrated durabillity checks
                        if (argument.CanCheckWeaponDurabillity)
                            Common.Durabillity.DoWeapon(this.character);

                        //Skill sucess
                        Predicate<Character> SendToCharacter = delegate(Character forwardTarget)
                        {
                            //Skill sucess
                            SMSG_OFFENSIVESKILL spkt = new SMSG_OFFENSIVESKILL();
                            spkt.SkillID = cpkt.SkillID;
                            spkt.SkillType = cpkt.SkillType;
                            spkt.TargetActor = cpkt.TargetActor;
                            spkt.SourceActor = this.character.id;
                            spkt.IsCritical = (forwardTarget.id == this.character.id || forwardTarget.id == target.id) ? (byte)argument.Result : (byte)7;
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
                            return true;
                        };

                        SendToCharacter(this.character);
                        Regiontree tree = this.character.currentzone.Regiontree;
                        foreach (Character forwardTarget in tree.SearchActors(this.character, SearchFlags.Characters))
                        {
                            if (forwardTarget.id == this.character.id) continue;
                            SendToCharacter(forwardTarget);
                        }

                        //Always update myself and oponent if it's character
                        LifeCycle.Update(this.character);
                        if (MapObject.IsPlayer(target))
                        {
                            LifeCycle.Update(target as Character);
                        }
                    }
                    else
                    {
                        //Skill failed
                        SMSG_OFFENSIVESKILLFAILED spkt = new SMSG_OFFENSIVESKILLFAILED();
                        spkt.SkillID = cpkt.SkillID;
                        spkt.SkillType = cpkt.SkillType;
                        spkt.SourceActor = this.character.id;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception processing the skill {0}", e.ToString());

                    //Skill failed
                    SMSG_OFFENSIVESKILLFAILED spkt = new SMSG_OFFENSIVESKILLFAILED();
                    spkt.SkillID = cpkt.SkillID;
                    spkt.SkillType = cpkt.SkillType;
                    spkt.SourceActor = this.character.id;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Occurs when toggling between skills (aka stances)
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLTOGGLE(CMSG_SKILLTOGLE cpkt)
        {
            lock (this.character.cooldowncollection)
            {
                try
                {
                    uint skillid = cpkt.SkillID;
                    byte skilltype = cpkt.SkillType;
                    SkillToggleEventArgs argument = null;

                    bool cancast = SkillToggleEventArgs.Create(skillid, this.character, this.character, out argument)
                              && argument.SpellInfo.casttime > -1 && (argument.SpellInfo.casttime == 0 || this.character._lastcastedskill == skillid)
                              && Environment.TickCount - this.character._lastcastedtick > 0
                              && (argument.SpellInfo.delay == 0 || !this.character.cooldowncollection.IsCoolDown(skillid))
                              && (argument.SpellInfo.maximumrange == 0 || argument.SpellInfo.IsInRangeOf((int)(Vector.GetDistance2D(this.character.Position, this.character.Position))))
                              && argument.SpellInfo.requiredWeapons[this.character.weapons.GetCurrentWeaponType()] == 1
                              && this.character.jlvl >= argument.SpellInfo.requiredJobs[this.character.job - 1]
                              && argument.SpellInfo.IsTarget(this.character, this.character);

                    if (cancast && argument.Use())
                    {
                        int delay = (int)(argument.SpellInfo.delay - ((character.stats.Dexterity * 2) + (character.stats.Concentration * 2)));
                        if (delay > 0) this.character.cooldowncollection.Add(skillid, delay);
                        this.character.cooldowncollection.Update();

                        SMSG_SKILLTOGLE spkt2 = new SMSG_SKILLTOGLE();
                        spkt2.SkillID = cpkt.SkillID;
                        spkt2.SkillType = cpkt.SkillType;
                        spkt2.SessionId = this.character.id;
                        spkt2.Toggle = argument.Failed;
                        this.Send((byte[])spkt2);

                        if (character._status.Updates > 0)
                        {
                            LifeCycle.Update(character);
                            character._status.Updates = 0;
                        }

                        Regiontree tree = this.character.currentzone.Regiontree;
                        foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
                        {
                            SMSG_OFFENSIVESKILL spkt = new SMSG_OFFENSIVESKILL();
                            spkt.Damage = argument.Damage;
                            spkt.SourceActor = this.character.id;
                            spkt.TargetActor = this.character.id;
                            spkt.SessionId = regionObject.id;
                            spkt.SkillID = cpkt.SkillID;
                            regionObject.client.Send((byte[])spkt);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Stance failed");
                        SMSG_SKILLTOGLE spkt2 = new SMSG_SKILLTOGLE();
                        spkt2.SkillID = cpkt.SkillID;
                        spkt2.SkillType = cpkt.SkillType;
                        spkt2.SessionId = this.character.id;
                        spkt2.Toggle = true;
                        this.Send((byte[])spkt2);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception processing the skill {0}", e.ToString());

                    //Skill failed
                    SMSG_OFFENSIVESKILLFAILED spkt = new SMSG_OFFENSIVESKILLFAILED();
                    spkt.SkillID = cpkt.SkillID;
                    spkt.SkillType = cpkt.SkillType;
                    spkt.SourceActor = this.character.id;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Occurs when using a item that uses a skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_ITEMTOGGLE(CMSG_ITEMTOGLE cpkt)
        {
            lock (this.character.cooldowncollection)
            {
                try
                {
                    Rag2Item item = this.character.container[cpkt.Index];
                    MapObject target;
                    uint skillid = cpkt.SkillID;
                    byte skilltype = cpkt.SkillType;
                    ItemSkillUsageEventArgs argument = null;

                    bool cancast = Regiontree.TryFind(cpkt.TargetActor, this.character, out target)
                              && ItemSkillUsageEventArgs.Create(item, this.character, target, out argument)
                              && argument.SpellInfo.casttime > -1 && (argument.SpellInfo.casttime == 0 || this.character._lastcastedskill == skillid)
                              && ((long)((uint)Environment.TickCount) - this.character._lastcastedtick) > 0
                              && (argument.SpellInfo.delay == 0 || !this.character.cooldowncollection.IsCoolDown(skillid))
                              && (argument.SpellInfo.maximumrange == 0 || argument.SpellInfo.IsInRangeOf((int)(Vector.GetDistance2D(this.character.Position, target.Position))))
                              && argument.SpellInfo.requiredWeapons[this.character.weapons.GetCurrentWeaponType()] == 1
                              && this.character.jlvl > argument.SpellInfo.requiredJobs[this.character.job - 1]
                              && argument.SpellInfo.IsTarget(this.character, target)
                              && item.count > 0;

                    if (cancast && argument.Use())
                    {
                        int delay = (int)(argument.SpellInfo.delay - ((character.stats.Dexterity * 2) + (character.stats.Concentration * 2)));
                        if (delay > 0) this.character.cooldowncollection.Add(skillid, delay);
                        this.character.cooldowncollection.Update();

                        int newLength = this.character.container[cpkt.Index].count - 1;
                        if (newLength > 0)
                        {
                            this.character.container[cpkt.Index].count = newLength;
                            SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                            spkt2.Amount = (byte)newLength;
                            spkt2.UpdateReason = 8;
                            spkt2.UpdateType = 4;
                            spkt2.Container = 2;
                            spkt2.SessionId = this.character.id;
                            spkt2.Index = cpkt.Index;
                            this.Send((byte[])spkt2);
                        }
                        else
                        {
                            this.character.container.RemoveAt(cpkt.Index);
                            SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                            spkt3.UpdateReason = 8;
                            spkt3.Container = 2;
                            spkt3.Index = cpkt.Index;
                            spkt3.SessionId = this.character.id;
                            this.Send((byte[])spkt3);
                        }

                        //Preprocess packet
                        SMSG_ITEMTOGGLE spkt = new SMSG_ITEMTOGGLE();
                        spkt.Container = cpkt.Container;
                        spkt.SkillMessage = (byte)argument.Result;
                        spkt.Index = cpkt.Index;
                        spkt.SkillID = cpkt.SkillID;
                        spkt.SkillType = cpkt.SkillType;
                        spkt.SourceActor = this.character.id;
                        spkt.TargetActor = cpkt.TargetActor;
                        spkt.Value = argument.Damage;

                        //Send packets in one-mighty blow
                        Regiontree tree = this.character.currentzone.Regiontree;
                        foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
                        {
                            if (character.client.isloaded == false || !Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position)) continue;
                            spkt.SessionId = target.id;
                            regionObject.client.Send((byte[])spkt);
                        }
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError("Error processing item skill");
                }
            }
        }

        /// <summary>
        /// Learns a new skill from a skillbook
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLS_LEARNFROMSKILLBOOK(CMSG_SKILLLEARN cpkt)
        {
            byte result = 1;
            try
            {
                Saga.Factory.Spells.Info spellinfo;
                Rag2Item item = character.container[cpkt.Index];
                if (item != null)
                {
                    //Helpers predicates
                    Predicate<Skill> FindSkill = delegate(Skill skill)
                    {
                        return skill.Id == item.info.skill;
                    };

                    Predicate<Skill> FindPreviousSkill = delegate(Skill skill)
                    {
                        return skill.Id == item.info.skill - 1;
                    };

                    //HELPER VARIABLES
                    uint baseskill = (item.info.skill / 100) * 100 + 1;
                    int newLength = character.container[cpkt.Index].count - 1;
                    List<Skill> learnedSpells = this.character.learnedskills;
                    Singleton.SpellManager.TryGetSpell(item.info.skill, out spellinfo);
                    Skill CurrentSkill = learnedSpells.FindLast(FindSkill);
                    Skill PreviousSkill = learnedSpells.FindLast(FindPreviousSkill);
                    bool IsBaseSkill = item.info.skill == baseskill;

                    //CHECK IF THE CURRENT JOB CAN LEARN THE SPELL
                    if (item.info.JobRequirement[this.character.job - 1] > this.character.jlvl)
                    {
                        result = (byte)Generalerror.ConditionsNotMet;
                    }
                    //CHECK IF WE ALREADY LEARNED THE SPELL
                    else if (CurrentSkill != null)
                    {
                        result = (byte)Generalerror.AlreadyLearntSkill;
                    }
                    //CHECK IF A PREVIOUS SKILL WAS FOUND
                    else if (!IsBaseSkill && PreviousSkill == null)
                    {
                        result = (byte)Generalerror.PreviousSkillNotFound;
                    }
                    //CHECK SKILL EXP
                    else if (PreviousSkill != null && PreviousSkill.Experience < PreviousSkill.info.maximumexperience)
                    {
                        result = (byte)Generalerror.NotEnoughSkillExperience;
                    }
                    else
                    {
                        //ADD A NEW SKILL
                        if (IsBaseSkill)
                        {
                            //Passive skill
                            bool canUse = Singleton.SpellManager.CanUse(this.character, spellinfo);
                            if (spellinfo.skilltype == 2 && canUse)
                            {
                                Singleton.Additions.ApplyAddition(spellinfo.addition, this.character);

                                int ActiveWeaponIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? this.character.weapons.SeconairyWeaponIndex : this.character.weapons.PrimaryWeaponIndex;
                                if (ActiveWeaponIndex < this.character.weapons.UnlockedWeaponSlots)
                                {
                                    Weapon weapon = this.character.weapons[ActiveWeaponIndex];
                                    if ((baseskill - 1) == weapon.Info.weapon_skill)
                                    {
                                        BattleStatus status = character._status;
                                        status.MaxWMAttack += (ushort)weapon.Info.max_magic_attack;
                                        status.MinWMAttack += (ushort)weapon.Info.min_magic_attack;
                                        status.MaxWPAttack += (ushort)weapon.Info.max_short_attack;
                                        status.MinWPAttack += (ushort)weapon.Info.min_short_attack;
                                        status.MaxWRAttack += (ushort)weapon.Info.max_range_attack;
                                        status.MinWRAttack += (ushort)weapon.Info.min_range_attack;
                                        status.Updates |= 2;
                                    }
                                }
                            }

                            Singleton.Database.InsertNewSkill(this.character, item.info.skill, spellinfo.maximumexperience);
                            CurrentSkill = new Skill();
                            CurrentSkill.info = spellinfo;
                            CurrentSkill.Id = item.info.skill;
                            CurrentSkill.Experience = spellinfo.maximumexperience;
                            learnedSpells.Add(CurrentSkill);
                        }
                        //UPDATE A OLD SKILL
                        else
                        {
                            //Passive skill
                            if (spellinfo.skilltype == 2)
                            {
                                Saga.Factory.Spells.Info oldSpellinfo;
                                Singleton.SpellManager.TryGetSpell(PreviousSkill.info.skillid, out oldSpellinfo);

                                bool canUseOld = Singleton.SpellManager.CanUse(this.character, oldSpellinfo);
                                bool canUseNew = Singleton.SpellManager.CanUse(this.character, spellinfo);

                                if (canUseOld)
                                {
                                    Singleton.Additions.DeapplyAddition(oldSpellinfo.addition, this.character);
                                }

                                if (canUseNew)
                                {
                                    Singleton.Additions.ApplyAddition(spellinfo.addition, this.character);
                                }
                            }

                            Singleton.Database.UpgradeSkill(this.character, PreviousSkill.info.skillid,
                                item.info.skill, spellinfo.maximumexperience);
                            PreviousSkill.info = spellinfo;
                            PreviousSkill.Id = item.info.skill;
                            PreviousSkill.Experience = spellinfo.maximumexperience;
                        }

                        SMSG_SKILLADD spkt2 = new SMSG_SKILLADD();
                        spkt2.Slot = 0;
                        spkt2.SkillId = item.info.skill;
                        spkt2.SessionId = this.character.id;
                        this.Send((byte[])spkt2);

                        if (newLength > 0)
                        {
                            this.character.container[cpkt.Index].count = newLength;
                            SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                            spkt.Amount = (byte)newLength;
                            spkt.UpdateReason = 8;
                            spkt.UpdateType = 4;
                            spkt.Container = 2;
                            spkt.SessionId = this.character.id;
                            spkt.Index = cpkt.Index;
                            this.Send((byte[])spkt);
                        }
                        else
                        {
                            this.character.container.RemoveAt(cpkt.Index);
                            SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                            spkt.UpdateReason = 8;
                            spkt.Container = 2;
                            spkt.Index = cpkt.Index;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        Common.Internal.CheckWeaponary(this.character);
                        Tasks.LifeCycle.Update(this.character);
                        result = 0;
                    }
                }
            }
            finally
            {
                //OUTPUT THE RESULT
                SMSG_SKILLLEARN spkt = new SMSG_SKILLLEARN();
                spkt.Result = result;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Tries to add the selected skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLS_ADDSPECIAL(CMSG_SETSPECIALSKILL cpkt)
        {
            byte result = 1;
            try
            {
                byte slot = cpkt.Slot;
                byte NearestSlot = (byte)((slot - (slot % 4)) + 4);
                Saga.Factory.Spells.Info info;

                if (Singleton.SpellManager.TryGetSpell(cpkt.SkillID, out info))
                {
                    //Generate skill
                    Skill skill = new Skill();
                    skill.info = info;
                    skill.Id = cpkt.SkillID;

                    //Skill must be a special skill
                    if (info.special == 0) return;

                    //Check if slots don't overlap
                    if (slot + info.special > NearestSlot) return;

                    //Check if slot isn't taken yet
                    for (int i = 0; i < info.special; i++)
                        if (this.character.SpecialSkills[slot + i] != null)
                            return;

                    //Set the bytes
                    for (int i = 0; i < info.special; i++)
                        this.character.SpecialSkills[slot + i] = skill;

                    //Set the result to okay

                    Common.Internal.CheckWeaponary(this.character);
                    result = 0;
                }
            }
            finally
            {
                SMSG_SETSPECIALSKILL spkt = new SMSG_SETSPECIALSKILL();
                spkt.Result = result;
                spkt.SkillID = cpkt.SkillID;
                spkt.Slot = cpkt.Slot;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Tries to remove the selected skill
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLS_REMOVESPECIAL(CMSG_REMOVESPECIALSKILL cpkt)
        {
            byte result = 1;
            try
            {
                Skill skill = this.character.SpecialSkills[cpkt.Slot];
                if (skill == null) return;
                if (skill.Id != cpkt.SkillID) return;

                for (int i = 0; i < skill.info.special; i++)
                {
                    this.character.SpecialSkills[cpkt.Slot + i] = null;
                }

                Common.Internal.CheckWeaponary(this.character);
                result = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                SMSG_REMOVESPECIALSKILL spkt = new SMSG_REMOVESPECIALSKILL();
                spkt.Result = result;
                spkt.SkillID = cpkt.SkillID;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Checks if the user has enought money
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SKILLS_REQUESTSPECIALSET(CMSG_WANTSETSPECIALLITY cpkt)
        {
            byte result = 0;
            uint req_zeny = 250;
            if (req_zeny > this.character.ZENY)
            {
                result = 1;
            }
            else
            {
                this.character.ZENY -= req_zeny;
                CommonFunctions.UpdateZeny(this.character);
            }

            SMSG_ADDSPECIALSKILLREPLY spkt = new SMSG_ADDSPECIALSKILLREPLY();
            spkt.result = result;
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }
    }
}