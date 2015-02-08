#define THREADING

using Saga.Enumarations;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Map.Utils.Structures;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Notifies the client to load start sending all pre-cached and yet to be
        /// processed packets.
        ///
        /// See the above method
        /// </summary>
        /// <remarks>
        /// Due the nature of this method we'll be invoking a notification on
        /// our "assumed" to be pending LoadMap method. We'll process all Moveable
        /// objects in the sightrange here.
        ///
        /// We cannot do this when we're precaching because suppose the client takes
        /// 10 minutes  to load, all the moveable monsters and characters can be gone
        /// already. And thus sending outdated information.
        /// </remarks>
        private void CM_CHARACTER_MAPLOADED()
        {
            try
            {
                #region Internal

                //Register on the new map
                this.character.currentzone.Regiontree.Subscribe(this.character);

                //GET NEW Z-POS
                float z = character.Position.z + 100;

                //Refresh Personal Requests
                CommonFunctions.RefreshPersonalRequests(this.character);
                Regiontree.UpdateRegion(this.character);
                isloaded = false;

                #endregion Internal

                #region Actor infromation

                {
                    /*
                     * Send actor information
                     *
                     * This packet makes a actor appear on the screen. Just like character information
                     * It defines automaticly which actor is your actor.
                     */

                    SMSG_ACTORINFO spkt = new SMSG_ACTORINFO();
                    spkt.ActorID = this.character.id;
                    spkt.X = this.character.Position.x;
                    spkt.Y = this.character.Position.y;
                    spkt.Z = this.character.Position.z;
                    spkt.race = this.character.race;
                    spkt.face = this.character.FaceDetails;
                    spkt.ActiveWeaponIndex = (byte)this.character.weapons.ActiveWeaponIndex;
                    spkt.InventoryContainerSize = (byte)this.character.container.Capacity;
                    spkt.StorageContainerSize = (byte)this.character.STORAGE.Capacity;
                    spkt.UnlockedWeaponCount = this.character.weapons.UnlockedWeaponSlots;
                    spkt.Unknown = 0;
                    spkt.PrimaryWeaponByIndex = this.character.weapons.PrimaryWeaponIndex;
                    spkt.SeccondairyWeaponByIndex = this.character.weapons.SeconairyWeaponIndex;
                    spkt.Name = this.character.Name;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                #endregion Actor infromation

                #region Battle Stats

                {
                    CommonFunctions.SendBattleStatus(this.character);
                }

                #endregion Battle Stats

                #region Extend Stats

                {
                    /*
                     * Sends over the status points attributes
                     *
                     */

                    SMSG_EXTSTATS spkt2 = new SMSG_EXTSTATS();
                    spkt2.base_stats_1 = character.stats.BASE;
                    spkt2.base_stats_2 = character.stats.CHARACTER;
                    spkt2.base_stats_jobs = character.stats.EQUIPMENT;
                    spkt2.base_stats_bonus = character.stats.ENCHANTMENT;
                    spkt2.statpoints = character.stats.REMAINING;
                    spkt2.SessionId = character.id;
                    this.Send((byte[])spkt2);
                }

                #endregion Extend Stats

                if (IsFirstTimeLoad)
                {
#if THREADING
                    WaitCallback FireTimeLoginCallback = delegate(object a)
                    {
#else
                    {
#endif

                        #region Equipment List

                        {
                            /*
                             * Sends a list of all Equiped Equipment
                             */

                            SMSG_EQUIPMENTLIST spkt = new SMSG_EQUIPMENTLIST();
                            for (int i = 0; i < 16; i++)
                            {
                                Rag2Item item = this.character.Equipment[i];
                                if (item != null) spkt.AddItem(item, item.active, i);
                            }

                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Equipment List

                        #region Weaponary List

                        {
                            /*
                             * Sends a list of all weapons
                             */

                            SMSG_WEAPONLIST spkt = new SMSG_WEAPONLIST();
                            spkt.SessionId = this.character.id;
                            for (int i = 0; i < 5; i++)
                            {
                                Weapon current = this.character.weapons[i];
                                spkt.AddWeapon(current);
                            }

                            this.Send((byte[])spkt);
                        }

                        #endregion Weaponary List

                        #region Unlock Weaponary

                        {
                            /*
                             * This packet unlocks the selected slots
                             */

                            SMSG_ADDITIONLIST spkt = new SMSG_ADDITIONLIST();
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Unlock Weaponary

                        #region Character Status

                        {
                            /*
                             * Send Character status
                             *
                             * This updates the characters status, this defines which job you're using
                             * the experience you have. The experience is expressed in absolute values
                             * so it doesn't represent a relative value for exp required.
                             *
                             * It also contains the definitions of how many LC, LP, HP, SP you have.
                             * Note: LC represents the amount of breath capacity.
                             */

                            SMSG_CHARSTATUS spkt = new SMSG_CHARSTATUS();
                            spkt.Job = this.character.job;
                            spkt.Exp = this.character.Cexp;
                            spkt.JobExp = this.character.Jexp;
                            spkt.LC = this.character._status.CurrentOxygen;
                            spkt.MaxLC = this.character._status.MaximumOxygen;
                            spkt.HP = this.character.HP;
                            spkt.MaxHP = this.character.HPMAX;
                            spkt.SP = this.character.SP;
                            spkt.MaxSP = this.character.SPMAX;
                            spkt.LP = this.character._status.CurrentLp;
                            spkt.MaxLP = 7;
                            spkt.FieldOfSight = 0;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Character Status

                        #region Battle Skills

                        {
                            /*
                             * Region Send all battle skills
                             *
                             * Also known as all active skills, heals, buffs included
                             */

                            List<Skill> list = this.character.learnedskills;
                            SMSG_BATTLESKILL battleskills = new SMSG_BATTLESKILL(list.Count);
                            foreach (Skill skill in list)
                                battleskills.AddSkill(skill.Id, skill.Experience);
                            battleskills.SessionId = this.character.id;
                            this.Send((byte[])battleskills);
                        }

                        #endregion Battle Skills

                        #region Special Skills

                        {
                            /*
                             * Sends over an list of special skills
                             *
                             */

                            SMSG_LISTSPECIALSKILLS spkt = new SMSG_LISTSPECIALSKILLS();
                            for (int i = 0; i < 16; i++)
                            {
                                Skill skill = character.SpecialSkills[i];
                                if (skill != null)
                                {
                                    spkt.AddSkill(skill.info.skillid, skill.Experience, (byte)i);
                                }
                            }
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Special Skills

                        #region Update Zeny

                        {
                            /*
                             * Send Money
                             *
                             * This sets the money of a player to a absolute value. For instance if you
                             * say 500, it would mean the player gets 500 rufi. There are no relative
                             * values for TakeMoney or GiveMoney.
                             */

                            SMSG_SENDZENY spkt = new SMSG_SENDZENY();
                            spkt.Zeny = this.character.ZENY;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Update Zeny

                        #region Send Quest List

                        {
                            SMSG_QUESTINFO spkt3 = new SMSG_QUESTINFO();
                            spkt3.SessionId = this.character.id;
                            foreach (QuestBase Quest in this.character.QuestObjectives)
                            {
                                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps =
                                    QuestBase.GetSteps(this.character, Quest.QuestId);
                                spkt3.AddQuest(Quest.QuestId, (byte)Steps.Count);
                                for (int i = 0; i < Steps.Count; i++)
                                {
                                    Saga.Quests.Objectives.ObjectiveList.StepInfo currentStep = Steps[i];
                                    uint nextstep = (i + 1 < Steps.Count) ? Steps[i + 1].StepId : 0;
                                    spkt3.AddQuestStep(currentStep.StepId, currentStep.State, nextstep, Quest.isnew);
                                }
                            }
                            this.Send((byte[])spkt3);

                            foreach (QuestBase Quest in this.character.QuestObjectives)
                            {
                                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps =
                                    QuestBase.GetSteps(this.character, Quest.QuestId);

                                for (int i = 0; i < Steps.Count; i++)
                                {
                                    Saga.Quests.Objectives.ObjectiveList.StepInfo currentStep = Steps[i];
                                    if (currentStep.State == 1)
                                    {
                                        Predicate<Saga.Quests.Objectives.ObjectiveList.SubStep> Findsubstep = delegate(Saga.Quests.Objectives.ObjectiveList.SubStep objective)
                                        {
                                            return objective.Quest == Quest.QuestId
                                            && objective.StepId == currentStep.StepId;
                                        };

                                        List<Quests.Objectives.ObjectiveList.SubStep> Substeps =
                                            this.character.QuestObjectives.Substeps.FindAll(Findsubstep);

                                        foreach (Saga.Quests.Objectives.ObjectiveList.SubStep substep in Substeps)
                                        {
                                            if (substep.current > 0)
                                            {
                                                SMSG_QUESTSUBSTEPUPDATE spkt = new SMSG_QUESTSUBSTEPUPDATE();
                                                spkt.Unknown = 1;
                                                spkt.SubStep = (byte)substep.SubStepId;
                                                spkt.StepID = substep.StepId;
                                                spkt.QuestID = substep.Quest;
                                                spkt.Amount = (byte)substep.current;
                                                spkt.SessionId = this.character.id;
                                                this.Send((byte[])spkt);
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        #endregion Send Quest List

                        #region Send Way Points

                        {
                            foreach (QuestBase Quest in this.character.QuestObjectives)
                            {
                                SMSG_SENDNAVIGATIONPOINT spkt2 = new SMSG_SENDNAVIGATIONPOINT();
                                spkt2.SessionId = this.character.id;
                                spkt2.QuestID = Quest.QuestId;

                                foreach (Saga.Quests.Objectives.ObjectiveList.Waypoint waypoint in QuestBase.UserGetWaypoints(this.character, Quest.QuestId))
                                {
                                    Predicate<MapObject> IsNpc = delegate(MapObject match)
                                    {
                                        return match.ModelId == waypoint.NpcId;
                                    };

                                    MapObject myObject = this.character.currentzone.Regiontree.SearchActor(IsNpc, SearchFlags.Npcs);
                                    if (myObject != null)
                                    {
                                        spkt2.AddPosition(waypoint.NpcId, myObject.Position.x, myObject.Position.y, myObject.Position.z);
                                    }
                                }
                                this.Send((byte[])spkt2);
                            }
                        }

                        #endregion Send Way Points

                        #region Addition List

                        {
                            SMSG_ADDITIONLIST spkt = new SMSG_ADDITIONLIST();
                            spkt.SessionId = this.character.id;
                            foreach (AdditionState state in character._additions)
                            {
                                if (state.CanExpire)
                                {
                                    spkt.Add(state.Addition, state.Lifetime);
                                }
                                else
                                {
                                    spkt.Add(state.Addition, 0);
                                }
                            }
                            this.Send((byte[])spkt);
                        }

                        #endregion Addition List

                        #region Scenario

                        {
                            QuestBase scenarioQuest = this.character.QuestObjectives.Quests[3];
                            if (scenarioQuest != null)
                            {
                                SMSG_INITIALIZESCENARIO spkt = new SMSG_INITIALIZESCENARIO();
                                spkt.Scenario1 = scenarioQuest.QuestId;
                                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> list;
                                if (this.character.QuestObjectives.ScenarioSteps.TryGetValue(scenarioQuest.QuestId, out list))
                                    if (list.Count > 0)
                                    {
                                        spkt.Scenario2 = list[0].StepId;
                                        spkt.StepStatus = 1;
                                        spkt.Enabled = 0;
                                    }

                                spkt.SessionId = this.character.id;
                                this.Send((byte[])spkt);
                            }
                            else
                            {
                                QuestBase qbase;
                                if (Singleton.Quests.TryFindScenarioQuest(1, out qbase))
                                {
                                    qbase.OnStart(character.id);
                                }
                            }
                        }

                        #endregion Scenario

                        #region Mail

                        {
                            int count = Singleton.Database.GetInboxUncheckedCount(this.character.Name);
                            SMSG_MAILARRIVED spkt = new SMSG_MAILARRIVED();
                            spkt.Amount = (uint)count;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }

                        #endregion Mail

                        #region Inventory List

                        {
                            SMSG_INVENTORYLIST spkt5 = new SMSG_INVENTORYLIST((byte)this.character.container.Count);
                            spkt5.SessionId = this.character.id;
                            foreach (Rag2Item c in this.character.container)
                                spkt5.AddItem(c);
                            this.Send((byte[])spkt5);
                        }

                        #endregion Inventory List

                        #region Friendlist

                        {
                            WaitCallback FriendlistLogin = delegate(object state)
                            {
                                //Look through all friends lists of logged on players and if they are friends then
                                // notify them that the user logged on.
                                SMSG_FRIENDSLIST_NOTIFYLOGIN spkt = new SMSG_FRIENDSLIST_NOTIFYLOGIN();
                                spkt.name = this.character.Name;
                                foreach (Character characterTarget in LifeCycle.Characters)
                                {
                                    lock (characterTarget)
                                    {
                                        if (characterTarget.id != this.character.id)
                                            if (character._friendlist.Contains(this.character.Name))
                                            {
                                                spkt.SessionId = characterTarget.id;
                                                characterTarget.client.Send((byte[])spkt);
                                            }
                                    }
                                }
                            };

                            ThreadPool.QueueUserWorkItem(FriendlistLogin);
                        }

                        #endregion Friendlist

#if THREADING
                    };
                    ThreadPool.QueueUserWorkItem(FireTimeLoginCallback);
#else
                    }
#endif
                }
                else
                {
                    //Always ubsubcribe from the respawn manager in case
                    //we forget to do it.
                    Tasks.Respawns.Unsubscribe(this.character);
                }

                #region Dynamic Objects

                {
                    try
                    {
                        Regiontree tree = this.character.currentzone.Regiontree;

#if THREADING
                        WaitCallback FindCharactersActors = delegate(object a)
                        {
#else
                        {
#endif
                            foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
                            {
                                try
                                {
                                    if (Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position))
                                    {
                                        regionObject.ShowObject(this.character);
                                        if (this.character.id != regionObject.id)
                                        {
                                            this.character.ShowObject(regionObject);
                                        }

                                        regionObject.Appears(this.character);
                                    }
                                }
                                catch (SocketException)
                                {
                                    Trace.WriteLine("Network eror");
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(e.Message);
                                }
                            }
#if THREADING
                        };
#else
                        }
#endif

#if THREADING
                        WaitCallback FindNpcActors = delegate(object a)
                        {
#else
                        {
#endif
                            foreach (MapObject regionObject in tree.SearchActors(this.character, SearchFlags.MapItems | SearchFlags.Npcs))
                            {
                                try
                                {
                                    if (Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position))
                                    {
                                        regionObject.ShowObject(this.character);
                                        regionObject.Appears(this.character);
                                    }
                                }
                                catch (SocketException)
                                {
                                    Trace.WriteLine("Network eror");
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(e.Message);
                                }
                            }

#if THREADING
                        };
#else
                        }
#endif

#if THREADING
                        ThreadPool.QueueUserWorkItem(FindCharactersActors);
                        ThreadPool.QueueUserWorkItem(FindNpcActors);
#endif
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                }

                #endregion Dynamic Objects

                #region Time Weather

                CommonFunctions.UpdateTimeWeather(this.character);

                #endregion Time Weather

                #region Map Info

                {
                    SMSG_SHOWMAPINFO spkt2 = new SMSG_SHOWMAPINFO();
                    spkt2.SessionId = this.character.id;
                    spkt2.ZoneInfo = this.character.ZoneInformation;
                    this.Send((byte[])spkt2);
                }

                #endregion Map Info

                #region Return Points

                {
                    /*
                     * Send Resturn points
                     * Return points define which map you'll be send to when you
                     * use your promise stone or die
                     */

                    WorldCoordinate lpos = this.character.lastlocation.map > 0 ? this.character.lastlocation : this.character.savelocation;
                    WorldCoordinate spos = this.character.savelocation;
                    if (spos.map == this.character.currentzone.Map)
                    {
                        this.character.stance = (byte)StancePosition.Stand;
                        SMSG_RETURNMAPLIST spkt = new SMSG_RETURNMAPLIST();
                        spkt.ToMap = lpos.map;
                        spkt.FromMap = this.character.currentzone.CathelayaLocation.map;
                        spkt.IsSaveLocationSet = (lpos.map > 0) ? (byte)1 : (byte)0;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                    else
                    {
                        this.character.stance = (byte)StancePosition.Stand;
                        SMSG_RETURNMAPLIST spkt = new SMSG_RETURNMAPLIST();
                        spkt.ToMap = spos.map;
                        spkt.FromMap = this.character.currentzone.CathelayaLocation.map;
                        spkt.IsSaveLocationSet = (spos.map > 0) ? (byte)1 : (byte)0;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                }

                #endregion Return Points

                #region Job Levels

                {
                    SMSG_JOBLEVELS spkt2 = new SMSG_JOBLEVELS();
                    spkt2.SessionId = this.character.id;
                    spkt2.jobslevels = this.character.CharacterJobLevel;
                    this.Send((byte[])spkt2);
                }

                #endregion Job Levels

                #region Change Actors state

                {
                    /*
                     * Change the actors state
                     *
                     * This is used to change the actors state to a non-active-battle position.
                     * Note we will not warp the player back on their x and y coords if they were dead.
                     * This would result in double loading a map. This should be handeld before processing
                     * the character.
                     */

                    this.character.stance = (byte)StancePosition.Reborn;
                    this.character.ISONBATTLE = false;
                    SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
                    spkt.ActorID = this.character.id;
                    spkt.SessionId = this.character.id;
                    spkt.Stance = this.character.stance;
                    spkt.State = (this.character.ISONBATTLE) ? (byte)1 : (byte)0;
                    this.Send((byte[])spkt);
                }

                #endregion Change Actors state

                #region Party Update

                if (this.character.sessionParty != null)
                {
                    foreach (Character partyTarget in this.character.sessionParty._Characters)
                    {
                        //Only process if we're on the same map
                        if (partyTarget.id != this.character.id)
                            if (partyTarget.map == this.character.map)
                            {
                                //Retrieve positions people that are loaded
                                SMSG_PARTYMEMBERLOCATION spkt2 = new SMSG_PARTYMEMBERLOCATION();
                                spkt2.Index = 1;
                                spkt2.ActorId = partyTarget.id;
                                spkt2.SessionId = this.character.id;
                                spkt2.X = (partyTarget.Position.x / 1000);
                                spkt2.Y = (partyTarget.Position.y / 1000);
                                this.Send((byte[])spkt2);

                                //Update the position of myself to loaded characters
                                SMSG_PARTYMEMBERLOCATION spkt3 = new SMSG_PARTYMEMBERLOCATION();
                                spkt3.Index = 1;
                                spkt3.ActorId = this.character.id;
                                spkt3.SessionId = partyTarget.id;
                                spkt3.X = (this.character.Position.x / 1000);
                                spkt3.Y = (this.character.Position.y / 1000);
                                partyTarget.client.Send((byte[])spkt3);
                            }
                    }
                }

                #endregion Party Update

                #region Static Objects

                try
                {
                    //SEND OVER ALL CHARACTERS THAT ARE ALWAYS VISIBLE
                    foreach (MapObject regionObject in this.character.currentzone.Regiontree.SearchActors(SearchFlags.StaticObjects))
                    {
                        Console.WriteLine("Show object: {0}", regionObject.ToString());
                        regionObject.ShowObject(this.character);
                        regionObject.Appears(this.character);
                    }
                }
                catch (Exception)
                {
                    //Do nothing
                }

                #endregion Static Objects
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.Close();
            }
            finally
            {
                this.character.ISONBATTLE = false;
                isloaded = true;
                IsFirstTimeLoad = false;
                this.character.LastPositionTick = Environment.TickCount;
            }
        }

        /// <summary>
        /// Checks for position updates.
        /// </summary>
        /// <remarks>
        /// First we'll check to see if we could see, actor x from
        /// our old position. After that we'll check if can see actor x
        /// from our new position. If we can see him, we'll forward our movement
        /// packet. If we can't see him we'll sent a actor disappearance packet.
        ///
        /// Once that's done, we'll check if we can see a non-former visible actor
        /// is now visible in our new position. Which will cause us to invoke a appearance
        /// packet.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_MOVEMENTSTART(CMSG_MOVEMENTSTART cpkt)
        {
            if (this.isloaded == false)
            {
                return;
            }

            //If character was previous walking/running
            if (this.character.IsMoving)
            {
                long time = (long)((uint)Environment.TickCount) - (long)((uint)this.character.LastPositionTick);

                //Sending packet to late
                if (time > 10000)
                {
                    CommonFunctions.Warp(this.character, this.character.Position, this.character.currentzone);
                    this.character.LastPositionTick = Environment.TickCount;
                    return;
                }
                else
                {
                    this.character.LastPositionTick = Environment.TickCount;
                }
            }
            else
            {
                this.character.LastPositionTick = Environment.TickCount;
            }

            //CALCULATE CURRENT MOVING SPEED
            ushort speed = (ushort)this.character._status.WalkingSpeed;
            if (cpkt.MovementType != 1)
            {
                if (speed > 0) speed /= 2;
            }

            //Get new actors for next-tick

            uint delay = cpkt.DelayTime;
            byte movement = cpkt.MovementType;
            float ax = cpkt.AccelerationX;
            float ay = cpkt.AccelerationY;
            float az = cpkt.AccelerationZ;

            Point oldP = this.character.Position;
            Point newP = new Point(cpkt.X, cpkt.Y, cpkt.Z);

            this.character.stance = (byte)StancePosition.Walk;
            this.character.Yaw = cpkt.Yaw;
            Regiontree tree = this.character.currentzone.Regiontree;

            try
            {
                foreach (MapObject regionObject in tree.SearchActors(this.character, SearchFlags.Npcs | SearchFlags.Characters | SearchFlags.MapItems, newP))
                {
                    bool CanSeePrev = Point.IsInSightRangeByRadius(regionObject.Position, oldP);
                    bool CanSeeNext = Point.IsInSightRangeByRadius(regionObject.Position, newP);
                    if (CanSeeNext && !CanSeePrev)
                    {
                        //Show object to other characters
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            this.character.ShowObject(current);
                        }

                        //Show object to self
                        regionObject.ShowObject(this.character);
                        regionObject.Appears(this.character);
                    }
                    else if (CanSeePrev && !CanSeeNext)
                    {
                        //Hide Object to other characters
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            this.character.HideObject(current);
                        }

                        //Hide Object to myself
                        regionObject.HideObject(this.character);
                        regionObject.Disappear(this.character);
                    }
                    else if (CanSeeNext && CanSeePrev)
                    {
                        //Update movment
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            if (current.client.isloaded == false) continue;

                            SMSG_MOVEMENTSTART spkt = new SMSG_MOVEMENTSTART();
                            spkt.SourceActorID = this.character.id;
                            spkt.Speed = speed;
                            spkt.X = oldP.x;
                            spkt.Y = oldP.y;
                            spkt.Z = oldP.z;
                            spkt.Yaw = this.character.Yaw;
                            spkt.MovementType = movement;
                            spkt.Delay0 = delay;
                            spkt.TargetActor = this.character._targetid;
                            spkt.AccelerationX = ax;
                            spkt.AccelerationY = ay;
                            spkt.AccelerationZ = az;
                            spkt.SessionId = current.id;
                            current.client.Send((byte[])spkt);
                        }

                        //Console.WriteLine("Disappear: {0}", regionObject.ToString());
                    }
                }
            }
            catch (Exception)
            {
                Trace.WriteLine("Error on local region");
            }
            finally
            {
                if (this.character.sessionParty != null)
                {
                    foreach (Character target in this.character.sessionParty._Characters)
                    {
                        try
                        {
                            if (target.id == this.character.id) continue;
                            SMSG_PARTYMEMBERLOCATION spkt3 = new SMSG_PARTYMEMBERLOCATION();
                            spkt3.Index = 1;
                            spkt3.ActorId = this.character.id;
                            spkt3.SessionId = target.id;
                            spkt3.X = (this.character.Position.x / 1000);
                            spkt3.Y = (this.character.Position.y / 1000);
                            target.client.Send((byte[])spkt3);
                        }
                        catch (SocketException)
                        {
                            Trace.WriteLine("Party network error");
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine("Unknown error");
                        }
                    }
                }
            }

            this.character.Position = newP;
            Regiontree.UpdateRegion(this.character);
            QuestBase.UserCheckPosition(this.character);
        }

        /// <summary>
        /// Checks for position updates.
        /// </summary>
        /// <remarks>
        /// First we'll check to see if we could see, actor x from
        /// our old position. After that we'll check if can see actor x
        /// from our new position. If we can see him, we'll forward our movement
        /// packet. If we can't see him we'll sent a actor disappearance packet.
        ///
        /// Once that's done, we'll check if we can see a non-former visible actor
        /// is now visible in our new position. Which will cause us to invoke a appearance
        /// packet.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_MOVEMENTSTOPPED(CMSG_MOVEMENTSTOPPED cpkt)
        {
            if (this.isloaded == false) return;
            //If character was previous walking/running
            if (!this.character.IsMoving)
            {
                this.character.LastPositionTick = Environment.TickCount;
                return;
            }
            else
            {
                //Sending packet to late
                int time = Environment.TickCount - this.character.LastPositionTick;
                if (time > 10000)
                {
                    CommonFunctions.Warp(this.character, this.character.Position, this.character.currentzone);
                    this.character.LastPositionTick = Environment.TickCount;
                    return;
                }
                else
                {
                    this.character.LastPositionTick = Environment.TickCount;
                }
            }

            // GENERATE NEW PACKETS, WE PREPROCESS
            SMSG_MOVEMENTSTOPPED spkt = new SMSG_MOVEMENTSTOPPED();
            spkt.ActorID = this.character.id;
            spkt.speed = cpkt.Speed;
            spkt.X = cpkt.X;
            spkt.Y = cpkt.Y;
            spkt.Z = cpkt.Z;
            spkt.yaw = cpkt.Yaw;
            spkt.TargetActor = this.character._targetid;
            spkt.DelayTime = cpkt.DelayTime;

            //Get new actors for next-tick
            Point oldP = this.character.Position;
            Point newP = new Point(cpkt.X, cpkt.Y, cpkt.Z);

            this.character.stance = (byte)StancePosition.Stand;
            this.character.Position = newP;
            Regiontree.UpdateRegion(this.character);
            Regiontree tree = this.character.currentzone.Regiontree;

            try
            {
                foreach (MapObject regionObject in tree.SearchActors(this.character, SearchFlags.Npcs | SearchFlags.Characters | SearchFlags.MapItems))
                {
                    bool CanSeePrev = Point.IsInSightRangeByRadius(regionObject.Position, oldP);
                    bool CanSeeNext = Point.IsInSightRangeByRadius(regionObject.Position, newP);
                    if (CanSeeNext && !CanSeePrev)
                    {
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            this.character.ShowObject(current);
                        }

                        regionObject.ShowObject(this.character);
                        regionObject.Appears(this.character);
                    }
                    else if (CanSeePrev && !CanSeeNext)
                    {
                        //Hide Object to other characters
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            this.character.HideObject(current);
                        }

                        //Hide Object to myself
                        regionObject.HideObject(this.character);
                        regionObject.Disappear(this.character);
                    }
                    else if (CanSeeNext && CanSeePrev)
                    {
                        if (MapObject.IsPlayer(regionObject))
                        {
                            Character current = (Character)regionObject;
                            if (current.client.isloaded == false) continue;
                            spkt.SessionId = current.id;
                            current.client.Send((byte[])spkt);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Trace.WriteLine("Error network code");
            }
            finally
            {
                if (this.character.sessionParty != null)
                {
                    foreach (Character target in this.character.sessionParty._Characters)
                    {
                        try
                        {
                            if (target.id == this.character.id) continue;
                            SMSG_PARTYMEMBERLOCATION spkt3 = new SMSG_PARTYMEMBERLOCATION();
                            spkt3.Index = 1;
                            spkt3.ActorId = this.character.id;
                            spkt3.SessionId = target.id;
                            spkt3.X = (this.character.Position.x / 1000);
                            spkt3.Y = (this.character.Position.y / 1000);
                            target.client.Send((byte[])spkt3);
                        }
                        catch (SocketException)
                        {
                            Trace.WriteLine("Network error");
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine("Unknown error");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Yaw
        /// </summary>
        /// <remarks>
        /// Yaw defines the angle of which a user is facing. This angle is expressed in
        /// yaw which has a maximum value of 65535 (size of ushort).
        ///
        /// And should be used to double check casting purposes. You're allowed to talk to a
        /// npc, attack a monster, use openbox skills only when delta angle meets ~35 degrees
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_UPDATEYAW(CMSG_SENDYAW cpkt)
        {
            if (this.isloaded == false) return;
            Regiontree tree = this.character.currentzone.Regiontree;
            this.character.Yaw = cpkt.Yaw;
            foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                if (regionObject.client.isloaded == true
                 && Point.IsInSightRangeByRadius(this.character.Position, regionObject.Position))
                {
                    SMSG_UPDATEYAW spkt = new SMSG_UPDATEYAW();
                    spkt.ActorID = this.character.id;
                    spkt.Yaw = this.character.Yaw;
                    spkt.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt);
                }
        }

        /// <summary>
        /// Makes your character jump in the air.
        /// </summary>
        private void CM_CHARACTER_JUMP()
        {
            SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
            spkt.ActorID = this.character.id;
            spkt.TargetActor = this.character._targetid;
            spkt.Stance = (byte)StancePosition.Jump;
            spkt.State = (byte)((this.character.ISONBATTLE == true) ? 1 : 0);

            foreach (MapObject myObject in this.character.currentzone.GetObjectsInRegionalRange(this.character))
                if (this.character.currentzone.IsInSightRangeBySquare(this.character.Position, myObject.Position))
                    if (MapObject.IsPlayer(myObject))
                    {
                        Character current = myObject as Character;
                        if (current.client.isloaded == true)
                        {
                            spkt.SessionId = current.id;
                            current.client.Send((byte[])spkt);
                        }
                    }
        }

        /// <summary>
        /// This packet is sent when the user goed underwater
        ///
        /// This invokes the starting of a the oxygen task, this task will
        /// take each seccond 1 LC from the Current LC state. This task will
        /// process all the required packets.
        /// </summary>
        private void CM_CHARACTER_DIVEDOWN()
        {
            this.character.IsDiving = true;
            SMSG_ACTORDIVE spkt = new SMSG_ACTORDIVE();
            spkt.SessionId = this.character.id;
            spkt.Direction = 0;
            spkt.Oxygen = this.character._status.CurrentOxygen;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// This packet is sent when the user reaches the surface/height is
        /// same as the water level.
        /// </summary>
        private void CM_CHARACTER_DIVEUP()
        {
            this.character.IsDiving = false;
            SMSG_ACTORDIVE spkt = new SMSG_ACTORDIVE();
            spkt.SessionId = this.character.id;
            spkt.Direction = 1;
            spkt.Oxygen = this.character._status.CurrentOxygen;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Occurs after a player fell from a large distance
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_FALL(CMSG_ACTORFALL cpkt)
        {
            if (this.isloaded == false) return;

            //HELPER VARIBALES
            uint value = cpkt.TargetActor / 8;
            ushort NewHP = (ushort)(this.character.HP - value);
            SMSG_TAKEDAMAGE spkt = new SMSG_TAKEDAMAGE();
            spkt.SessionId = this.character.id;

            //Update last hp tick
            this.character.LASTHP_TICK = Environment.TickCount;

            //Get the falling reason
            if (value >= this.character.HP)
                spkt.Reason = (byte)TakeDamageReason.FallenDead;
            else if (NewHP < (this.character.HPMAX / 10))
                spkt.Reason = (byte)TakeDamageReason.Survive;
            else
                spkt.Reason = (byte)TakeDamageReason.Falling;

            //Update some stuff
            bool isdead = value > this.character._status.CurrentHp;
            if (isdead)
            {
                spkt.Damage = this.character._status.CurrentHp;
                this.Send((byte[])spkt);
                this.character.stance = (byte)StancePosition.Dead;
                this.character.UpdateReason |= 1;
                this.character._status.CurrentHp = 0;
                this.character.OnDie();
                this.character._status.Updates |= 1;
                LifeCycle.Update(this.character);
            }
            else
            {
                spkt.Damage = value;
                this.Send((byte[])spkt);
                this.character._status.CurrentHp = NewHP;
                Common.Skills.UpdateAddition(this.character, 201, 30000);
                this.character._status.Updates |= 1;
                LifeCycle.Update(this.character);
            }

            //Update life cycle
            LifeCycle.Update(this.character);
            Regiontree tree = this.character.currentzone.Regiontree;
            foreach (Character sTarget in tree.SearchActors(SearchFlags.Characters))
            {
                if (isdead)
                    Common.Actions.UpdateStance(sTarget, this.character);
            }
        }

        /// <summary>
        /// Changes the state of the player.
        /// </summary>
        /// <remarks>
        /// This functions is invoked when the user changes his/hers p
        /// players state. For example when the players is laying down
        ///or is sitting down. This packet is called.
        ///
        /// When a player is going to sit down, the regen bonusses are
        /// changed. The regeneration bonussed are since the 26dec patch
        /// 2007:
        ///
        /// Players battle stance:    0-HP 0.35MSP-SP
        /// Player standing:         15-HP 0.35MSP-SP
        /// Player sitting:          15-HP 0.35MSP-SP
        /// Player laying:           15-HP 0.35MSP-SP
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_CHANGESTATE(CMSG_ACTORCHANGESTATE cpkt)
        {
            if (cpkt.Stance > 12) return;
            //Checks if the old position was a valentine lay

            Regiontree tree = this.character.currentzone.Regiontree;
            bool LeaveLove = (this.character.stance == 9 || this.character.stance == 10);
            if (LeaveLove)
            {
                Character target;
                if (LifeCycle.TryGetById(this.character._ShowLoveDialog, out target))
                {
                    //Update the stance
                    switch ((StancePosition)target.stance)
                    {
                        case StancePosition.VALENTINE_SIT: target.stance = (byte)StancePosition.Sit; break;
                        case StancePosition.VALENTINE_LAY: target.stance = (byte)StancePosition.Lie; break;
                        default: target.stance = (byte)StancePosition.Stand; break;
                    }

                    //Release the showlove
                    this.character._ShowLoveDialog = 0;

                    //Reset the stance
                    foreach (Character current in tree.SearchActors(this.character, SearchFlags.Characters))
                    {
                        if (current.client.isloaded == false || !Point.IsInSightRangeByRadius(current.Position, this.character.Position)) continue;
                        SMSG_ACTORCHANGESTATE spkt2 = new SMSG_ACTORCHANGESTATE();
                        spkt2.ActorID = target.id;
                        spkt2.State = (byte)((target.ISONBATTLE) ? 1 : 0);
                        spkt2.Stance = target.stance;
                        spkt2.TargetActor = target._targetid;
                        spkt2.SessionId = current.id;
                        current.client.Send((byte[])spkt2);
                    }
                }
            }

            //Update my information
            this.character.stance = cpkt.Stance;
            this.character.ISONBATTLE = cpkt.State > 0;

            //Create packet
            SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
            spkt.ActorID = this.character.id;
            spkt.State = cpkt.State;
            spkt.Stance = cpkt.Stance;
            spkt.TargetActor = cpkt.TargetActor;
            foreach (Character current in tree.SearchActors(this.character, SearchFlags.Characters))
            {
                if (current.client.isloaded == false) continue;
                spkt.SessionId = current.id;
                current.client.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Character uses a portal
        /// </summary>
        /// <remarks>
        ///This function is invoked when a players used a portal
        ///to warp to another place. Because we made a common function of the
        ///warp, it safe for use to invoke that.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_USEPORTAL(CMSG_USEPORTAL cpkt)
        {
            try
            {
                Saga.Factory.Portals.Portal portal;
                if (Singleton.portal.TryFind(cpkt.PortalID, this.character.map, out portal))
                {
                    CommonFunctions.Warp(this.character, portal.mapID, portal.destinaton);
                }
                else
                {
                    Trace.TraceInformation("Portal not found: {0}", cpkt.PortalID);
                    SMSG_PORTALFAIL spkt = new SMSG_PORTALFAIL();
                    spkt.Reason = 1;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
            }
            catch (Exception)
            {
                SMSG_PORTALFAIL spkt = new SMSG_PORTALFAIL();
                spkt.Reason = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Changes the stats of a character
        /// </summary>
        /// <remarks>
        /// This function updates the characters stats with
        /// (read as synchorinisation) the client.
        ///
        /// Momentairly we do no checking if for cheating or
        /// packet injection. Todo: do checks for packet
        /// injection.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_STATPOINTUPDATE(CMSG_STATUPDATE cpkt)
        {
            Saga.PrimaryTypes.CharacterStats.Stats stats = this.character.stats.CHARACTER;
            int maxwantedstats = cpkt.Strength + cpkt.Dextericty + cpkt.Intellect + cpkt.Concentration + cpkt.PointsLeft;
            int maxcharstats = stats.strength + stats.dexterity + stats.intelligence + stats.concentration + this.character.stats.REMAINING;

            //Cannot alter the stats is starting stats isnt equal with ending stats.
            //Prevents hacking etc to gain more stats.
            if (maxwantedstats != maxcharstats) return;

            lock (character._status)
            {
                //Update strength
                character._status.MaxPAttack -= (ushort)(2 * stats.strength);
                character._status.MinPAttack -= (ushort)(1 * stats.strength);
                character._status.MaxHP -= (ushort)(10 * stats.strength);
                character._status.MaxPAttack += (ushort)(2 * cpkt.Strength);
                character._status.MaxPAttack += (ushort)(1 * cpkt.Strength);
                character._status.MaxHP += (ushort)(10 * cpkt.Strength);
                stats.strength = cpkt.Strength;

                //Update Dextericty
                character._status.BasePHitrate -= (ushort)(1 * stats.dexterity);
                character._status.BasePHitrate += (ushort)(1 * cpkt.Dextericty);
                stats.dexterity = cpkt.Dextericty;

                //Update Intellect
                character._status.MaxMAttack -= (ushort)(6 * stats.intelligence);
                character._status.MinMAttack -= (ushort)(3 * stats.intelligence);
                character._status.BaseRHitrate -= (ushort)(1 * stats.intelligence);
                character._status.MaxMAttack += (ushort)(6 * cpkt.Intellect);
                character._status.MinMAttack += (ushort)(3 * cpkt.Intellect);
                character._status.BaseRHitrate += (ushort)(1 * cpkt.Intellect);
                stats.intelligence = cpkt.Intellect;

                //Update Concentration
                character._status.MaxRAttack -= (ushort)(4 * stats.concentration);
                character._status.MinRAttack -= (ushort)(2 * stats.concentration);
                character._status.BasePHitrate -= (ushort)(2 * stats.concentration);
                character._status.MaxRAttack += (ushort)(4 * cpkt.Concentration);
                character._status.MinRAttack += (ushort)(2 * cpkt.Concentration);
                character._status.BasePHitrate += (ushort)(2 * cpkt.Concentration);
                stats.concentration = cpkt.Concentration;
            }

            //Update points left
            this.character.stats.REMAINING = cpkt.PointsLeft;

            //Update the client
            CommonFunctions.SendExtStats(this.character);
            CommonFunctions.UpdateCharacterInfo(this.character, 0);
            CommonFunctions.SendBattleStatus(this.character);
        }

        /// <summary>
        /// Is invoked when a character chooses one of the options in
        /// their homepoint button.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_RETURNTOHOMEPOINT(CMSG_SENDHOMEPOINT cpkt)
        {
            if (this.character.savelocation.map > 0 && cpkt.Type == 0)
            {
                WorldCoordinate world = this.character.savelocation;
                this.character.HP = 1;
                this.character.ISONBATTLE = false;
                CommonFunctions.Warp(this.character, world.map, world.coords);
                this.character.lastlocation = this.character.savelocation;
            }
            else
            {
                //this.character.promise_map = this.character.currentzone.CathelayaMap;
                //this.character.promiseposition = this.character.currentzone.CathelayaLocation;

                WorldCoordinate world = this.character.currentzone.CathelayaLocation;
                this.character.HP = 1;
                this.character.ISONBATTLE = false;
                CommonFunctions.Warp(this.character, world.map, world.coords);
                this.character.lastlocation = this.character.savelocation;
            }
        }

        /// <summary>
        /// Occurs when requesting to show love to somebody. It forwards a packet to the
        /// desired end-user.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_SHOWLOVE(CMSG_SHOWLOVE cpkt)
        {
            Character target;
            bool isfound = LifeCycle.TryGetById(cpkt.ActorId, out target);

            if (isfound && target._ShowLoveDialog == 0)
            {
                SMSG_REQUESTSHOWLOVE spkt = new SMSG_REQUESTSHOWLOVE();
                spkt.ActorID = this.character.id;
                spkt.SessionId = target.id;
                target._ShowLoveDialog = this.character.id;
                this.character._ShowLoveDialog = target.id;

                if (target.client != null)
                    target.client.Send((byte[])spkt);
            }
            else if (isfound && target._ShowLoveDialog > 0)
            {
                SMSG_RESPONSESHOWLOVE spkt = new SMSG_RESPONSESHOWLOVE();
                spkt.Result = 3;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //PLAYER DOES NOT EXISTS
            else
            {
                SMSG_RESPONSESHOWLOVE spkt = new SMSG_RESPONSESHOWLOVE();
                spkt.Result = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when confirming or decling to show love you've
        /// recieve from a person.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_CONFIRMSHOWLOVE(CMSG_SHOWLOVECONFIRM cpkt)
        {
            Character target;
            if (LifeCycle.TryGetById(this.character._ShowLoveDialog, out target))
            {
                if (cpkt.Response == 0)
                {
                    SMSG_ACTORCHANGESTATE spkt3 = new SMSG_ACTORCHANGESTATE();
                    spkt3.ActorID = this.character.id;
                    spkt3.State = 0;
                    spkt3.Stance = (byte)StancePosition.VALENTINE_LAY;
                    spkt3.TargetActor = target.id;

                    SMSG_ACTORCHANGESTATE spkt2 = new SMSG_ACTORCHANGESTATE();
                    spkt2.ActorID = target.id;
                    spkt2.State = 0;
                    spkt2.Stance = (byte)StancePosition.VALENTINE_SIT;
                    spkt2.TargetActor = this.character.id;

                    foreach (MapObject myObject in this.character.currentzone.GetObjectsInRegionalRange(this.character))
                    {
                        if (this.character.currentzone.IsInSightRangeBySquare(this.character.Position, myObject.Position))
                        {
                            if (MapObject.IsPlayer(myObject))
                            {
                                Character current = myObject as Character;
                                if (current.client != null && current.client.isloaded == true)
                                {
                                    spkt3.SessionId = current.id;
                                    spkt2.SessionId = current.id;
                                    current.client.Send((byte[])spkt3);
                                    current.client.Send((byte[])spkt2);
                                }
                            }
                        }
                    }
                }
                else
                {
                    character._ShowLoveDialog = 0;
                }

                if (target.client != null)
                {
                    SMSG_RESPONSESHOWLOVE spkt = new SMSG_RESPONSESHOWLOVE();
                    spkt.Result = cpkt.Response;
                    spkt.SessionId = target.id;
                    target.client.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Requests to change the job.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_JOBCHANGE(CMSG_CHANGEJOB cpkt)
        {
            JobChangeCollection collection = this.character.Tag as JobChangeCollection;
            if (collection == null || !collection.IsJobAvailable(cpkt.Job))
            {
                //Job change failed
                SMSG_JOBCHANGED spkt = new SMSG_JOBCHANGED();
                spkt.Job = this.character.job;
                spkt.Result = 1;
                spkt.SessionId = this.character.id;
                spkt.SourceActor = this.character.id;
                this.Send((byte[])spkt);
                return;
            }
            else if (collection.GetJobTrasferFee(cpkt.Job) > this.character.ZENY)
            {
                //Not enough money
                Common.Errors.GeneralErrorMessage(this.character, (uint)Generalerror.NotEnoughMoney);

                SMSG_JOBCHANGED spkt = new SMSG_JOBCHANGED();
                spkt.Job = this.character.job;
                spkt.Result = 1;
                spkt.SessionId = this.character.id;
                spkt.SourceActor = this.character.id;
                this.Send((byte[])spkt);
                return;
            }
            else
            {
                this.character.ZENY -= collection.GetJobTrasferFee(cpkt.Job);
                CommonFunctions.UpdateZeny(this.character);
            }

            //Helper variables
            List<int> EnabledEquipment = new List<int>();
            List<int> DisabledEquipment = new List<int>();
            bool ChangeWeapon = cpkt.ChangeWeapon == 1;
            bool IsActiveWeapon = this.character.weapons.IsActiveSlot(cpkt.WeaponSlot);
            Weapon selectedWeapon = this.character.weapons[cpkt.WeaponSlot];

            #region Update Character Information

            lock (this.character)
            {
                //Change the job and joblevel
                int hpbefore = Singleton.CharacterConfiguration.CalculateMaximumHP(this.character);
                int spbefore = Singleton.CharacterConfiguration.CalculateMaximumSP(this.character);
                this.character.CharacterJobLevel[this.character.job] = this.character.jlvl;
                this.character.jlvl = this.character.CharacterJobLevel[cpkt.Job];
                this.character.job = cpkt.Job;
                this.character.Jexp = Singleton.experience.FindRequiredJexp(this.character.jlvl);
                int hpafter = Singleton.CharacterConfiguration.CalculateMaximumHP(this.character);
                int spafter = Singleton.CharacterConfiguration.CalculateMaximumSP(this.character);
                this.character._status.CurrentHp += (ushort)(hpbefore - hpafter);
                this.character._status.CurrentSp += (ushort)(spbefore - spafter);
                this.character._status.Updates |= 1;
            }

            #endregion Update Character Information

            #region Refresh Weapon

            //Deapply current weapon info
            if (ChangeWeapon && IsActiveWeapon && selectedWeapon != null)
            {
                BattleStatus status = this.character._status;
                status.MaxWMAttack -= (ushort)selectedWeapon.Info.max_magic_attack;
                status.MinWMAttack -= (ushort)selectedWeapon.Info.min_magic_attack;
                status.MaxWPAttack -= (ushort)selectedWeapon.Info.max_short_attack;
                status.MinWPAttack -= (ushort)selectedWeapon.Info.min_short_attack;
                status.MaxWRAttack -= (ushort)selectedWeapon.Info.max_range_attack;
                status.MinWRAttack -= (ushort)selectedWeapon.Info.min_range_attack;
                status.Updates |= 2;

                //Reapplies alterstone additions
                for (int i = 0; i < 8; i++)
                {
                    uint addition = selectedWeapon.Slots[i];
                    if (addition > 0)
                    {
                        Singleton.Additions.DeapplyAddition(addition, character);
                    }
                }
            }

            #endregion Refresh Weapon

            #region Refresh Weapon

            if (ChangeWeapon && selectedWeapon != null)
            {
                Singleton.Weapons.ChangeWeapon(cpkt.Job, cpkt.PostFix, selectedWeapon);
                SMSG_WEAPONCHANGE spkt = new SMSG_WEAPONCHANGE();
                spkt.Auge = selectedWeapon._augeskill;
                spkt.SessionId = this.character.id;
                spkt.Suffix = cpkt.PostFix;
                spkt.Index = cpkt.WeaponSlot;
                spkt.WeaponType = (byte)selectedWeapon._type;
                this.Send((byte[])spkt);
            }

            #endregion Refresh Weapon

            #region Refresh Skills

            {
                //Remove all skills
                foreach (Skill skill in this.character.learnedskills)
                {
                    //Remove the skill
                    SMSG_SKILLREMOVE spkt = new SMSG_SKILLREMOVE();
                    spkt.Unknown = skill.info.skilltype;
                    spkt.SessionId = this.character.id;
                    spkt.SkillId = skill.info.skillid;
                    this.Send((byte[])spkt);

                    //Save only experience if it's maxed-out.
                    Singleton.Database.UpdateSkill(this.character, skill.Id,
                        (skill.Experience == skill.info.maximumexperience) ? skill.info.maximumexperience : 0);

                    //Deapply passive skills
                    bool canUse = Singleton.SpellManager.CanUse(this.character, skill.info);
                    if (skill.info.skilltype == 2 && canUse)
                        Singleton.Additions.DeapplyAddition(skill.info.addition, character);
                }

                //Remove all learned skills in an instant
                this.character.learnedskills.Clear();
                Singleton.Database.LoadSkills(this.character);

                //Retrieve job speciafic skills
                foreach (Skill skill in this.character.learnedskills)
                {
                    //Add the skill
                    SMSG_SKILLADD spkt = new SMSG_SKILLADD();
                    spkt.SessionId = this.character.id;
                    spkt.SkillId = skill.info.skillid;
                    spkt.Slot = 0;
                    this.Send((byte[])spkt);

                    //Deapply passive skills
                    bool canUse = Singleton.SpellManager.CanUse(this.character, skill.info);
                    if (skill.info.skilltype == 2 && canUse)
                        Singleton.Additions.ApplyAddition(skill.info.addition, character);
                }
            }

            #endregion Refresh Skills

            #region Refresh Weapon

            //Apply current weapon info
            if (ChangeWeapon && IsActiveWeapon && selectedWeapon != null)
            {
                //Apply status
                BattleStatus status = this.character._status;
                status.MaxWMAttack += (ushort)selectedWeapon.Info.max_magic_attack;
                status.MinWMAttack += (ushort)selectedWeapon.Info.min_magic_attack;
                status.MaxWPAttack += (ushort)selectedWeapon.Info.max_short_attack;
                status.MinWPAttack += (ushort)selectedWeapon.Info.min_short_attack;
                status.MaxWRAttack += (ushort)selectedWeapon.Info.max_range_attack;
                status.MinWRAttack += (ushort)selectedWeapon.Info.min_range_attack;
                status.Updates |= 2;

                //Reapplies alterstone additions
                for (int i = 0; i < 8; i++)
                {
                    uint addition = selectedWeapon.Slots[i];
                    if (addition > 0)
                    {
                        Singleton.Additions.ApplyAddition(addition, character);
                    }
                }
            }

            #endregion Refresh Weapon

            #region Refresh Equipment

            {
                //Disable equipment
                for (int i = 0; i < 16; i++)
                {
                    //Verify if item exists
                    Rag2Item item = this.character.Equipment[i];
                    if (item == null || item.info == null) continue;

                    //Verify if the item changed states
                    bool Active = item.active == 1;
                    bool NewActive = this.character.jlvl >= item.info.JobRequirement[cpkt.Job - 1];
                    if (Active == NewActive) continue;
                    item.active = (byte)((NewActive == true) ? 1 : 0);

                    //Adjust the item
                    SMSG_ITEMADJUST spkt = new SMSG_ITEMADJUST();
                    spkt.Container = 1;
                    spkt.Function = 5;
                    spkt.Slot = (byte)i;
                    spkt.SessionId = this.character.id;
                    spkt.Value = item.active;
                    this.Send((byte[])spkt);

                    //Deapply additions
                    if (NewActive)
                    {
                        EnabledEquipment.Add(i);
                        Singleton.Additions.ApplyAddition(item.info.option_id, character);
                    }
                    else
                    {
                        DisabledEquipment.Add(i);
                        Singleton.Additions.DeapplyAddition(item.info.option_id, character);
                    }
                }

                //Update other stats
                //Common.Internal.CheckWeaponary(this.character);
                //CommonFunctions.UpdateCharacterInfo(this.character, 0);
                //CommonFunctions.SendBattleStatus(this.character);
            }

            #endregion Refresh Equipment

            #region Refresh Appereance

            {
                Regiontree tree = this.character.currentzone.Regiontree;
                foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                {
                    SMSG_JOBCHANGED spkt = new SMSG_JOBCHANGED();
                    spkt.SessionId = regionObject.id;
                    spkt.SourceActor = this.character.id;
                    spkt.Job = cpkt.Job;
                    regionObject.client.Send((byte[])spkt);

                    if (regionObject.id != this.character.id)
                    {
                        if (IsActiveWeapon)
                        {
                            uint auge = (character.FindRequiredRootSkill(selectedWeapon.Info.weapon_skill)) ? selectedWeapon._augeskill : 0;
                            SMSG_SHOWWEAPON spkt2 = new SMSG_SHOWWEAPON();
                            spkt2.ActorID = this.character.id;
                            spkt2.AugeID = auge;
                            spkt2.SessionId = regionObject.id;
                            regionObject.client.Send((byte[])spkt2);
                        }

                        foreach (byte i in EnabledEquipment)
                        {
                            Rag2Item item = this.character.Equipment[i];
                            SMSG_CHANGEEQUIPMENT spkt2 = new SMSG_CHANGEEQUIPMENT();
                            spkt2.SessionId = regionObject.id;
                            spkt2.ActorID = this.character.id;
                            spkt2.Slot = i;
                            spkt2.ItemID = item.info.item;
                            spkt2.Dye = item.dyecolor;
                            regionObject.client.Send((byte[])spkt2);
                        }

                        foreach (byte i in DisabledEquipment)
                        {
                            SMSG_CHANGEEQUIPMENT spkt2 = new SMSG_CHANGEEQUIPMENT();
                            spkt2.SessionId = regionObject.id;
                            spkt2.ActorID = this.character.id;
                            spkt2.Slot = i;
                            spkt2.ItemID = 0;
                            spkt2.Dye = 0;
                            regionObject.client.Send((byte[])spkt2);
                        }
                    }
                }
            }

            #endregion Refresh Appereance

            #region Refresh Party

            {
                //Process party members
                SMSG_PARTYMEMBERJOB spkt = new SMSG_PARTYMEMBERJOB();
                spkt.Job = cpkt.Job;
                spkt.SessionId = this.character.id;
                spkt.ActorId = this.character.id;
                spkt.Index = 1;

                SMSG_PARTYMEMBERJLVL spkt2 = new SMSG_PARTYMEMBERJLVL();
                spkt2.Jvl = this.character.jlvl;
                spkt2.ActorId = this.character.id;
                spkt2.Index = 1;

                if (this.character.sessionParty != null)
                {
                    foreach (Character target in this.character.sessionParty.GetCharacters())
                    {
                        //Send over Job change
                        spkt.SessionId = target.id;
                        target.client.Send((byte[])spkt);

                        //Send over jlvl change
                        spkt2.SessionId = target.id;
                        target.client.Send((byte[])spkt2);
                    }
                }
            }

            #endregion Refresh Party

            #region Refresh LifeCycle

            Tasks.LifeCycle.Update(this.character);

            #endregion Refresh LifeCycle
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_CHARACTER_EXPENSIVESFORJOBCHANGE(CMSG_SELECTJOB cpkt)
        {
            //Get's the job change collection
            JobChangeCollection collection = this.character.Tag as JobChangeCollection;
            if (collection != null)
            {
                SMSG_SENDMONEYFORJOB spkt = new SMSG_SENDMONEYFORJOB();
                spkt.SessionId = this.character.id;
                spkt.Zeny = collection.GetJobTrasferFee(cpkt.Job);
                this.Send((byte[])spkt);
            }

            /*
            int Price = 200;
            try
            {
                Price += (character.CharacterJobLevel[cpkt.Job] * 300);
                List<uint> Skills = Singleton.Database.GetJobSpeciaficSkills(this.character, cpkt.Job);
                for (int i = 0; i < Skills.Count; i++)
                {
                    int Lvl = (int)(Skills[i] % 100);
                    Price += ((Lvl - 1) * 100);
                }
            }
            finally
            {
                SMSG_SENDMONEYFORJOB spkt = new SMSG_SENDMONEYFORJOB();
                spkt.SessionId = this.character.id;
                spkt.Zeny = (uint)Price;
                this.Send((byte[])spkt);
            }*/
        }
    }
}