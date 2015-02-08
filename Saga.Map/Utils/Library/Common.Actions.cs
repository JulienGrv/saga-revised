using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common
{
    public static class Actions
    {
        public static void Kick(Character character)
        {
            //Closes the client
            character.client.Close();
        }

        public static void SetGmLevel(int gmlevel, Character source, Character target)
        {
            if (source == target)
            {
                CommonFunctions.Broadcast(source, source, "You cannot alter your own gmlevel");
            }
            else if (gmlevel >= source.GmLevel)
            {
                CommonFunctions.Broadcast(source, source, "You give a person a higher gm leven than yourself");
            }
            else if (target.GmLevel > source.GmLevel)
            {
                CommonFunctions.Broadcast(source, source, "You give a edit superior gm");
            }
            else
            {
                target.GmLevel = (byte)gmlevel;
                CommonFunctions.Broadcast(source, source, string.Format("Target: {0} is now gm {1}", target.Name, gmlevel));
                Trace.TraceInformation("Gm level was revoked by: {0} on: {1} to {9}", source.Name, target.Name, gmlevel);
            }
        }

        #region Kaftra

        /// <summary>
        /// Shows the storage of a given character.
        /// </summary>
        /// <param name="target"></param>
        public static void ShowStorage(Character target)
        {
            SMSG_STORAGELIST spkt = new SMSG_STORAGELIST();
            foreach (KeyValuePair<byte, Rag2Item> pair in target.STORAGE.GetAllItems())
                spkt.AddItem(pair.Value, pair.Key);
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }

        #endregion Kaftra

        #region Misc

        /// <summary>
        /// Selects a given actor
        /// </summary>
        /// <param name="target"></param>
        /// <param name="item"></param>
        public static void SelectActor(Character target, MapObject item)
        {
            ISelectAble current = item as ISelectAble;
            if (current != null)
            {
                target._targetid = item.id;
                SMSG_ACTORSELECTION spkt = new SMSG_ACTORSELECTION();
                spkt.SessionId = target.id;
                spkt.MaxHP = current.HPMAX;
                spkt.HP = current.HP;
                spkt.MaxSP = current.SPMAX;
                spkt.SP = current.SP;
                spkt.SourceActorID = target.id;
                spkt.TargetActorID = target._targetid;
                target.client.Send((byte[])spkt);
                target._target = target;
            }
        }

        /// <summary>
        /// Selects a given actor
        /// </summary>
        /// <param name="target"></param>
        /// <param name="item"></param>
        public static void SelectActor(Character target, Actor item)
        {
            target._targetid = item.id;
            SMSG_ACTORSELECTION spkt = new SMSG_ACTORSELECTION();
            spkt.SessionId = target.id;
            spkt.MaxHP = item.HPMAX;
            spkt.HP = item.HP;
            spkt.MaxSP = item.SPMAX;
            spkt.SP = item.SP;
            spkt.SourceActorID = target.id;
            spkt.TargetActorID = target._targetid;
            target.client.Send((byte[])spkt);
            target._target = item;
        }

        public static void UpdateStance(Character target, Actor actor)
        {
            if (actor != null)
            {
                Console.WriteLine("Updating stance: {0} {1}", actor.stance, actor.ModelId);
                SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
                spkt.Stance = actor.stance;
                spkt.State = 0;
                spkt.SessionId = target.id;
                spkt.ActorID = actor.id;
                spkt.TargetActor = actor._targetid;
                target.client.Send((byte[])spkt);
            }
        }

        public static void UpdateStance(Actor actor)
        {
            if (actor == null) return;
            Regiontree tree = actor.currentzone.Regiontree;
            foreach (Character target in tree.SearchActors(actor, SearchFlags.Characters))
            {
                if (Point.IsInSightRangeByRadius(target.Position, actor.Position))
                {
                    SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
                    spkt.Stance = actor.stance;
                    spkt.State = 0;
                    spkt.SessionId = target.id;
                    spkt.ActorID = actor.id;
                    spkt.TargetActor = actor._targetid;
                    target.client.Send((byte[])spkt);
                }
            }
        }

        public static void UpdateIcon(Character target, BaseMob mob)
        {
            SMSG_ACTORUPDATEICON spkt = new SMSG_ACTORUPDATEICON();
            spkt.ActorID = mob.id;
            spkt.SessionId = target.id;
            spkt.Icon = (byte)mob.ComputeIcon(target);
            target.client.Send((byte[])spkt);
        }

        internal static void UpdateMemberHp(Character source, Character target)
        {
            SMSG_PARTYMEMBERHPINFO spkt = new SMSG_PARTYMEMBERHPINFO();
            spkt.Index = 1;
            spkt.ActorId = target.id;
            spkt.Hp = target.HP;
            spkt.HpMax = target.HPMAX;
            spkt.SessionId = source.id;
            source.client.Send((byte[])spkt);
        }

        internal static void UpdateMemberSp(Character source, Character target)
        {
            SMSG_PARTYMEMBERSPINFO spkt = new SMSG_PARTYMEMBERSPINFO();
            spkt.Index = 1;
            spkt.ActorId = target.id;
            spkt.Sp = target.SP;
            spkt.SpMax = target.SPMAX;
            spkt.SessionId = source.id;
            source.client.Send((byte[])spkt);
        }

        public static void OpenMenu(Character target, MapObject source, uint dialogScript, DialogType type, params byte[] icons)
        {
            target._target = source;
            target.ActiveDialog = dialogScript;
            SMSG_NPCCHAT spkt = new SMSG_NPCCHAT();
            spkt.Icons = icons;
            spkt.Unknown = (byte)type;
            spkt.Script = dialogScript;
            spkt.ActorID = source.id;
            spkt.SessionId = target.id;
            spkt.Unknown2 = 1;
            target.client.Send((byte[])spkt);
        }

        public static void OpenMenu(Character target, MapObject source, uint dialogScript, DialogType type, params DialogType[] icons)
        {
            byte[] dialogs = new byte[icons.Length];
            for (int i = 0; i < icons.Length; i++)
                dialogs[i] = (byte)icons[i];
            OpenMenu(target, source, dialogScript, type, dialogs);
        }

        public static void OpenSubmenu(Character target, MapObject source, uint dialogScript, DialogType type, params byte[] icon)
        {
            target._target = source;
            target.ActiveDialog = dialogScript;
            SMSG_NPCCHAT spkt = new SMSG_NPCCHAT();
            spkt.Icons = icon;
            spkt.Unknown = (byte)type;
            spkt.Script = dialogScript;
            spkt.ActorID = source.id;
            spkt.SessionId = target.id;
            spkt.Unknown2 = 2;
            target.client.Send((byte[])spkt);
        }

        public static void OpenLocationGuide(Character target, uint menu)
        {
            SMSG_NPCASKLOCATION spkt = new SMSG_NPCASKLOCATION();
            spkt.Script = menu;
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }

        #endregion Misc

        public static void ShowEvents(Character target)
        {
            SMSG_EVENTINFO spkt = new SMSG_EVENTINFO();
            spkt.SessionId = target.id;
            foreach (byte eventid in Singleton.EventManager.GetVisibleEvents())
            {
                Predicate<byte> FindParticipatingEvent = delegate(byte evid)
                {
                    return eventid == evid;
                };

                bool hasparticpated = target.ParticipatedEvents.Exists(FindParticipatingEvent);
                spkt.Add(eventid, hasparticpated);
            }
            target.client.Send((byte[])spkt);
        }
    }
}