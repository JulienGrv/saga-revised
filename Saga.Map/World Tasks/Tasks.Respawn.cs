using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.Tasks
{
    /// <summary>
    /// Performs a respawn task.
    /// </summary>
    /// <remarks>
    /// A normal monster respawns in approx 3 minutes.
    /// A character is forces to respawn after 30 minutes unless they press
    /// the homepoint button before the time expires.
    /// </remarks>
    public static class Respawns
    {
        #region Private Members

        /// <summary>
        /// Queue for respawn objects
        /// </summary>
        private static Queue<Respawn> respawns = new Queue<Respawn>();

        /// <summary>
        /// Queue for respawn players
        /// </summary>
        private static List<Respawn> respawns_players = new List<Respawn>();

        /// <summary>
        /// Respawns a monster or mapobject
        /// </summary>
        /// <param name="c"></param>
        private static void SendRespawn(MapObject c)
        {
            bool isnpc = MapObject.IsNpc(c);
            bool ismapitem = MapObject.IsMapItem(c);

            c.OnDeregister();
            c.OnSpawn();
            c.OnRegister();

            Regiontree tree = c._currentzone.Regiontree;
            if (isnpc)
                foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                {
                    Actor actor = c as Actor;
                    actor.stance = (byte)StancePosition.Reborn;
                    SMSG_NPCINFO spkt2 = new SMSG_NPCINFO(0);
                    spkt2.ActorID = actor.id;
                    spkt2.HP = actor.HPMAX;
                    spkt2.SP = actor.SPMAX;
                    spkt2.MaxHP = actor.HPMAX;
                    spkt2.MaxSP = actor.SPMAX;
                    spkt2.NPCID = actor.ModelId;
                    spkt2.X = actor.Position.x;
                    spkt2.Y = actor.Position.y;
                    spkt2.Z = actor.Position.z;
                    spkt2.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt2);
                    c.Appears(regionObject);
                }
            else if (ismapitem)
                foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                {
                    MapItem item = c as MapItem;
                    SMSG_ITEMINFO spkt2 = new SMSG_ITEMINFO();
                    spkt2.ActorID = item.id;
                    spkt2.CanInteract = item.IsInteractable(regionObject);
                    spkt2.IsActive = item.IsHighlighted(regionObject);
                    spkt2.NPCID = item.ModelId;
                    spkt2.X = item.Position.x;
                    spkt2.Y = item.Position.y;
                    spkt2.Z = item.Position.z;
                    spkt2.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt2);
                    c.Appears(regionObject);
                }
        }

        /// <summary>
        /// Forces a full respawn of the player.
        /// </summary>
        /// <param name="c"></param>
        /// <remarks>
        /// This happens after approx 30 minutes after the
        /// player still didn't press the button.
        /// </remarks>
        private static void ForceFullRespawnPlayer(Character c)
        {
            WorldCoordinate world = c.savelocation;
            c.stance = (byte)StancePosition.Reborn;
            CommonFunctions.Warp(c, world.map, world.coords);
        }

        #endregion Private Members

        #region Internal Members

        /// <summary>
        /// Process all mapspawns
        /// </summary>
        internal static void Process()
        {
            //PROCESS ALL MOBS RESPAWNS
            Respawn current;
            while (respawns.Count > 0)
            {
                current = respawns.Peek();
                if (Environment.TickCount - current.Tick < 4600) break;

                current.Object.currentzone = current.Zone;
                current.Zone.Regiontree.Subscribe(current.Object);
                SendRespawn(current.Object);
                respawns.Dequeue();
            }
            //PROCESS ALL PLAYER RESPAWNS (FORCES RESPAWN AFTER 10 MINUTES)
            while (respawns_players.Count > 0)
            {
                Respawn c = respawns_players[0];
                Character d = c.Object as Character;
                if (d.stance != (byte)StancePosition.Dead) break;
                if (Environment.TickCount - c.Tick < 600000) break;
                ForceFullRespawnPlayer(c.Object as Character);
                respawns_players.RemoveAt(0);
            }
        }

        #endregion Internal Members

        #region Public Members

        public static void Subscribe(MapObject c)
        {
            //IF THE SELECTED OBJECT IS A PLAYER
            if (MapObject.IsPlayer(c))
            {
                respawns_players.Add(new Respawn(c, c.currentzone));
            }
            else if (c.CanRespawn == true)
            {
                c.currentzone.Regiontree.Unsubscribe(c);
                respawns.Enqueue(new Respawn(c, c.currentzone));
            }
            else
            {
                c.currentzone.Regiontree.Unsubscribe(c);
            }
        }

        public static void Unsubscribe(MapObject c)
        {
            if (MapObject.IsPlayer(c))
            {
                lock (respawns_players)
                {
                    for (int i = 0; i < respawns_players.Count; i++)
                    {
                        if (respawns_players[i].Object == c)
                        {
                            respawns_players.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
        }

        #endregion Public Members

        #region Nested

        private class Respawn
        {
            public readonly int Tick;
            public readonly MapObject Object;
            public readonly Zone Zone;

            public Respawn(MapObject corpse, Zone zone)
            {
                this.Tick = Environment.TickCount;
                this.Object = corpse;
                this.Zone = zone;
            }
        }

        #endregion Nested
    }
}