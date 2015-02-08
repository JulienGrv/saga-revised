using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

namespace Saga.Tasks
{
    /// <summary>
    /// Task that helps clear all corpses.
    /// </summary>
    public static class Corpses
    {
        #region Private Members

        /// <summary>
        /// List of corpses that should be processed.
        /// </summary>
        private static Queue<Corpse> corpses = new Queue<Corpse>();

        /// <summary>
        /// Sends a corpse delete message to every visible
        /// player.
        /// </summary>
        /// <param name="c"></param>
        private static void SendCorpseClear(MapObject c)
        {
            Regiontree tree = c.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(c, SearchFlags.Characters))
            {
                try
                {
                    SMSG_ACTORDELETE spkt = new SMSG_ACTORDELETE();
                    spkt.ActorID = c.id;
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                }
                catch (ObjectDisposedException)
                {
                    //Do nothing if the object is disposed
                }
            }
        }

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Subscribes an MapObject as an Corpse
        /// </summary>
        /// <param name="c"></param>
        public static void Subscribe(MapObject c)
        {
            corpses.Enqueue(new Corpse(c));
        }

        #endregion Public Members

        #region Internal Members

        /// <summary>
        /// Processes all our corpses
        /// </summary>
        internal static void Process()
        {
            Corpse current;
            while (corpses.Count > 0)
            {
                current = corpses.Peek();
                if (Environment.TickCount - current.Tick < 17600) break;

                current.Object.currentzone.Regiontree.Unsubscribe(current.Object);
                Respawns.Subscribe(current.Object);
                SendCorpseClear(current.Object);
                corpses.Dequeue();
            }
        }

        #endregion Internal Members

        #region Nested Classes

        /// <summary>
        /// Corpse class defines of when you're dead.
        /// </summary>
        private class Corpse
        {
            /// <summary>
            /// Tick when you dead
            /// </summary>
            public readonly int Tick;

            /// <summary>
            /// Target of the corpse
            /// </summary>
            public readonly MapObject Object;

            /// <summary>
            /// Creates a new corpse object
            /// </summary>
            /// <param name="corpse">Target who to corpse</param>
            public Corpse(MapObject corpse)
            {
                this.Tick = Environment.TickCount;
                this.Object = corpse;
            }
        }

        #endregion Nested Classes
    }
}