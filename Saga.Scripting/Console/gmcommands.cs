using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Saga.Scripting
{
    public static partial class Console
    {
        /// <summary>
        /// Warps yourself to a specified map
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!pos</example>
        [GmAttribute("pos", 1, "(.*?)")]
        public static void Position(Character character, Match match)
        {
            //Return the character's position
            CommonFunctions.Broadcast(character, character, string.Format("{0} on map {1}", character.Position, character.map));
        }

        /// <summary>
        /// Get's the current number of connected character
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!worldload</example>
        [GmAttribute("who", 0, "(.*?)")]
        public static void Worldload(Character character, Match match)
        {
            //Return the character's position
            string message = string.Format(CultureInfo.InvariantCulture, "{0} Personnes connectées ", Tasks.LifeCycle.Count);
            CommonFunctions.Broadcast(character, character, message);
        }

        /// <summary>
        /// Warps yourself to the specified map
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!warptomap 1</example>
        [GmAttribute("warptomap", 5, "^(\\d{1,3})$")]
        public static void GmWarptomap(Character character, Match match)
        {
            byte map = Convert.ToByte(match.Groups[1].Value);
            CommonFunctions.Warp(character, map);
        }

        /// <summary>
        /// Warps yourself to the specified map
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!warptomap 1</example>
        [GmAttribute("go", 5, "^(\\d{1,3})$")]
        public static void Gmgo(Character character, Match match)
        {
            Point coord;
            switch (Convert.ToInt16(match.Groups[1].Value))
            {
                case 1:
                    coord.x = -12955f;
                    coord.y = -50262;
                    coord.z = 17208;
                    byte map = Convert.ToByte(1);
                    CommonFunctions.Warp(character, map, coord);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Warps yourself to another player
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!pjump charactername</example>
        [GmAttribute("pjump", 5, "^(.+?)$")]
        public static void PlayerJump(Character character, Match match)
        {
            Character characterTarget;
            if (Tasks.LifeCycle.TryGetByName(match.Groups[1].Value, out characterTarget))
            {
                CommonFunctions.Warp
                (
                    character,
                    characterTarget.Position,
                    characterTarget.currentzone
                );
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "character was not found");
            }
        }

        /// <summary>
        /// Warps a player to yourself
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!pcall charactername</example>
        [GmAttribute("pcall", 10, "^(.+?)$")]
        public static void PlayerCall(Character character, Match match)
        {
            Character characterTarget;
            if (Tasks.LifeCycle.TryGetByName(match.Groups[1].Value, out characterTarget))
            {
                CommonFunctions.Warp
                (
                    characterTarget,
                    character.Position,
                    character.currentzone
                );
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "character was not found");
            }
        }

        /// <summary>
        /// Set's the walking speed of your character
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!speed 300</example>
        [GmAttribute("speed", 10, "^(\\d{1,5})$")]
        public static void Speed(Character character, Match match)
        {
            character.Status.WalkingSpeed = ushort.Parse(match.Groups[1].Value);
        }

        /// <summary>
        /// Resets the global gametime
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!time 20-60-60</example>
        [GmAttribute("time", 10, @"(\d{1,2})-(\d{1,2})-(\d{1,2})")]
        public static void Time(Character character, Match match)
        {
            lock (Tasks.WorldTime.Time)
            {
                Tasks.WorldTime.Time[0] = byte.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                Tasks.WorldTime.Time[1] = byte.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                Tasks.WorldTime.Time[2] = byte.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            }

            ThreadStart AsyncOperation = delegate()
            {
                //Resend npc to actor
                foreach (Character mcharacter in Tasks.LifeCycle.Characters)
                {
                    CommonFunctions.UpdateTimeWeather(mcharacter);
                }
            };

            Thread thread = new Thread(AsyncOperation);
            thread.Start();
        }

        /// <summary>
        /// Shows when the next maintaince is scheduled
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!showmaintenance</example>
        [GmAttribute("showmaintenance", 10, "(.*?)")]
        public static void ShowMaintenance(Character character, Match match)
        {
            string message = Tasks.Maintenance.IsScheduled ? string.Format("Maintaince is scheduled on: {0}", Tasks.Maintenance.NextSceduledMaintenance.ToShortTimeString()) : "Maintaince is not scheduled";
            CommonFunctions.Broadcast(character, message);
        }

        /// <summary>
        /// Kicks a player with a specified name
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!kick charactername</example>
        [GmAttribute("kick", 30, "^(.+?)$")]
        public static void Kick(Character character, Match match)
        {
            Character ncharacter;
            if (Tasks.LifeCycle.TryGetByName(match.Groups[1].Value, out ncharacter))
            {
                Common.Actions.Kick(ncharacter);
                if (ncharacter.id != character.id)
                    CommonFunctions.Broadcast(character, character, "Character is kicked");
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "Character with that name is not online");
            }
        }

        /**
         * <summary>
         * This command kills the current monster if it is a monster.
         * </summary>
         */

        [GmAttribute("kill", 30, "(.*?)")]
        public static void Kill(Character character, Match match)
        {
            Regiontree tree = character.currentzone.Regiontree;
            MapObject regionObject;
            if (character.SelectedTarget > 0 && Regiontree.TryFind(character.SelectedTarget, character, out regionObject) && regionObject is Monster)
            {
                /*****
                //Make the Object die
                *****/
                Actor btarget = regionObject as Actor;
                btarget.Status.CurrentHp = 0;
                btarget.OnDie(character);
                character.OnEnemyDie(regionObject);
                btarget.stance = 7;
                btarget.Status.Updates |= 1;

                /*****
                //Forcing updates to all surrounding objects (flusing)
                *****/
                Predicate<Character> SendToCharacter = delegate(Character forwardTarget)
                {
                    //Process some general updates
                    if (forwardTarget.SelectedTarget == regionObject.id)
                        Common.Actions.SelectActor(forwardTarget, regionObject as Actor);
                    Common.Actions.UpdateStance(forwardTarget, regionObject as Actor);
                    Common.Actions.UpdateIcon(forwardTarget, regionObject as BaseMob);
                    return true;
                };

                /*****
                //Forcing updates to all surrounding objects (searching)
                *****/
                SendToCharacter(character);
                foreach (Character forwardTarget in tree.SearchActors(character, SearchFlags.Characters))
                {
                    if (forwardTarget.id == character.id) continue;
                    SendToCharacter(forwardTarget);
                }

                /*****
                //Update lifecycle
                *****/
                LifeCycle.Update(character);
                if (MapObject.IsPlayer(regionObject))
                {
                    LifeCycle.Update(regionObject as Character);
                }

                /***
                 * Check Quest
                 ***/
                if (MapObject.IsNpc(btarget))
                {
                    Quests.QuestBase.UserEliminateTarget(btarget.ModelId, character);
                }
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "No target selected or incorrect target");
            }
        }

        /**
         * <summary>
         * This command forces to start a quest. This is purely for debugging purposes.
         * </summary>
         */

        [GmAttribute("qstart", 30, "^\\d*$")]
        public static void QStart(Character character, Match match)
        {
            try
            {
                uint quest = uint.Parse(match.Groups[1].Value);
                QuestBase Quest;
                if (Singleton.Quests.TryFindQuests(quest, out Quest) &&
                    Quest.OnStart(character.id) < 0)
                {
                    QuestBase.InvalidateQuest(Quest, character);
                }
                else
                {
                    Quest.CheckQuest(character);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        /// <summary>
        /// Spawns a npc or mapobject with the specified id. Use 1 as first digit to load a
        /// npc and 2 to load a item.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!spawn 0 10407</example>
        [GmAttribute("spawn", 30, @"^(\d) (\d+) (\d+)$")]
        public static void Spawn(Character character, Match match)
        {
            uint model = uint.Parse(match.Groups[2].Value);
            byte b = byte.Parse(match.Groups[1].Value);
            int nb = Convert.ToInt16(match.Groups[3].Value);

            if (b == 0)
            {
                MapObject instance = null;
                System.Threading.WaitCallback callback = delegate(object state)
                {
                    Singleton.Templates.UnspawnInstance(instance);
                };

                //if (Singleton.Templates.SpawnNpcInstance(model, character.Position, character.Yaw, character.currentzone, false, out instance))
                //{
                //QueedTask timer = new QueedTask(callback, instance, 300000);
                //}

                if (nb > 0)
                {
                    for (int i = 1; i <= nb; i++)
                    {
                        Singleton.Templates.SpawnNpcInstance(model, character.Position, character.Yaw, character.currentzone, false, out instance);
                    }
                }
            }
            else if (b == 1)
            {
                MapObject instance;
                Point newPosition = character.Position;
                newPosition.x += (float)(200 * Math.Cos(character.Yaw.rotation * (Math.PI / 32768)));
                newPosition.y += (float)(200 * Math.Sin(character.Yaw.rotation * (Math.PI / 32768)));
                Singleton.Templates.SpawnItemInstance(model, newPosition, character.Yaw, character.currentzone, out instance);
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "Invalid argument");
            }
        }

        /// <summary>
        /// Unspawns a the selected actor of the character
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!unspawn</example>
        [GmAttribute("unspawn", 30, @"(.*?)")]
        public static void Unspawn(Character character, Match match)
        {
            uint model = uint.Parse(match.Groups[2].Value);
            byte b = byte.Parse(match.Groups[1].Value);

            if (character.Target != null && !(character.Target is Character))
            {
                Singleton.Templates.UnspawnInstance(character.Target);
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "No actor selected or actor is a player");
            }
        }

        /// <summary>
        /// Broadcast an annouchement to all users
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!b message to broadcast</example>
        [GmAttribute("b", 33, "^(.+?)$")]
        public static void Broadcast(Character character, Match match)
        {
            CommonFunctions.SystemMessage(character, match.Groups[1].Value);
        }

        /// <summary>
        /// Mutes the specified character for an hour.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!mute charactername</example>
        [GmAttribute("mute", 33, "^(.+?)$")]
        public static void ChatMute(Character character, Match match)
        {
            Character ncharacter;
            if (Tasks.LifeCycle.TryGetByName(match.Groups[1].Value, out ncharacter))
            {
                //Addition for chat mute
                if (character.GmLevel > ncharacter.GmLevel)
                {
                    Common.Skills.UpdateAddition(ncharacter, 1301, 3600000);
                    CommonFunctions.Broadcast(character, character, "Character is muted for one hour");
                }
                else
                {
                    CommonFunctions.Broadcast(character, character, "Cannot mute a supperior gm");
                }
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "Character with that name is not online");
            }
        }

        /// <summary>
        /// Set's the temp gm level of the character. The information will be lost
        /// after the client closes.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!setgmlvl 21 charactername</example>
        [GmAttribute("setgmlvl", 50, "^(\\d) (.+?)$")]
        public static void SetGmLevel(Character character, Match match)
        {
            Character ncharacter;
            if (Tasks.LifeCycle.TryGetByName(match.Groups[1].Value, out ncharacter))
            {
                int gmlevel = int.Parse(match.Groups[1].Value);
                Common.Actions.SetGmLevel(gmlevel, character, ncharacter);
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "Character with that name is not online");
            }
        }

        /// <summary>
        /// Gives an item to the event rewards list.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!item 30000 1 charactername</example>
        [GmAttribute("item", 55, "^(\\d+) (\\d{1,3}) (.+?)$")]
        public static void GiveItem(Character character, Match match)
        {
            uint item = uint.Parse(match.Groups[1].Value);
            byte count = byte.Parse(match.Groups[2].Value);
            byte maxstack = 0;
            if (Singleton.Item.TryGetStackcount(item, out maxstack))
            {
                if (count <= maxstack)
                {
                    if (!Singleton.Database.GiveItemReward(match.Groups[3].Value, item, count))
                    {
                        CommonFunctions.Broadcast(character, character, "Player was not found");
                    }
                }
                else
                {
                    CommonFunctions.Broadcast(character, character, "Stackcount larger than allowed");
                }
            }
            else
            {
                CommonFunctions.Broadcast(character, character, "Item not found");
            }
        }

        /// <summary>
        /// Repopulates the world.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!repopulate</example>
        [GmAttribute("repopulate", 90, "(.*?)")]
        public static void ClearNpc(Character character, Match match)
        {
            ThreadStart AsyncOperation = delegate()
            {
                //Clear all mobs
                foreach (Zone zone in Singleton.Zones.HostedZones())
                {
                    zone.Clear();
                }

                //Respawns world objects (npc & quests)
                Singleton.QuestBoardSpawnManager.Reload();

                //Respawn npc
                Singleton.NpcSpawnManager.Reload();

                //Resend npc to actor
                foreach (Character mcharacter in Tasks.LifeCycle.Characters)
                {
                    Regiontree tree = mcharacter.currentzone.Regiontree;
                    foreach (MapObject regionObject in tree.SearchActors(mcharacter, Saga.Enumarations.SearchFlags.Npcs | Saga.Enumarations.SearchFlags.MapItems | Saga.Enumarations.SearchFlags.StaticObjects))
                    {
                        if (Point.IsInSightRangeByRadius(mcharacter.Position, regionObject.Position))
                        {
                            regionObject.ShowObject(character);
                            regionObject.Appears(character);
                        }
                    }
                }
            };

            Thread thread = new Thread(AsyncOperation);
            thread.Start();
        }

        /// <summary>
        /// Kicks all player connected to the server except yourself
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!kickall</example>
        [GmAttribute("kickall", 90, "(.*?)")]
        public static void KickAll(Character character, Match match)
        {
            ThreadStart AsyncOperation = delegate()
            {
                //Resend npc to actor

                List<Character> characters = new List<Character>();
                characters.AddRange(Tasks.LifeCycle.Characters);
                foreach (Character mcharacter in characters)
                {
                    if (mcharacter.id == character.id) continue;
                    Common.Actions.Kick(mcharacter);
                }
            };

            Thread thread = new Thread(AsyncOperation);
            thread.Start();
        }

        /// <summary>
        /// Schedules the next maintaince. Use a date in the past to cancel it.
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!schedulemaintenance 2008-07-31 18:00</example>
        [GmAttribute("schedulemaintenance", 90, @"(\d\d\d\d)-(\d\d)-(\d\d) (\d\d):(\d\d)")]
        public static void ScheduleMaintenance(Character character, Match match)
        {
            DateTime newdate = DateTime.ParseExact(match.Groups[0].Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            if (newdate < DateTime.Now)
            {
                string message = "Maintaince is canceled";
                Tasks.Maintenance.IsScheduled = false;
                Tasks.Maintenance.NextSceduledMaintenance = newdate;
                CommonFunctions.Broadcast(character, character, message);
            }
            else
            {
                string message = string.Format("Maintaince is scheduled on: {0}", newdate);
                Tasks.Maintenance.IsScheduled = true;
                Tasks.Maintenance.NextSceduledMaintenance = newdate;
                CommonFunctions.SystemMessage(character, character, message);
            }
        }

        /// <summary>
        /// Forces the garbage collector to run
        /// </summary>
        /// <param name="character">Character calling the command</param>
        /// <param name="match">Regex match</param>
        /// <example>!gc</example>
        [GmAttribute("gc", 99, "(.*?)")]
        public static void GarbageCollector(Character character, Match match)
        {
            GC.Collect(0);
        }
    }
}