using Saga.Map;
using Saga.Packets;
using Saga.Quests;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class LootCollection : IDisposable
    {
        #region Private Properties

        private bool isdisposed;
        private bool islooting;
        private uint lootleader;
        private Rag2Collection lootlist;

        #endregion Private Properties

        #region Public Properties

        public Rag2Collection Lootlist
        {
            get
            {
                return lootlist;
            }
        }

        public bool CanLoot
        {
            get
            {
                return islooting;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return isdisposed;
            }
        }

        public uint LootLeader
        {
            get
            {
                return lootleader;
            }
            internal set
            {
                lootleader = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public bool CanOpen(Character character)
        {
            PartySession party = character.sessionParty;
            if (party == null)
            {
                return character.id == this.lootleader;
            }
            else if (party.LootSettings == 1)
            {
                return character.id == this.lootleader;
            }
            else if (party.LootSettings == 2)
            {
                bool isPartyMember = party.IsMemberOfParty(character.id);
                return isPartyMember && islooting;
            }
            else if (party.LootSettings == 3)
            {
                return character.id == this.lootleader;
            }
            else
            {
                return false;
            }
        }

        public void Open(Character character, MapObject owner)
        {
            character.Tag = this;
            SMSG_SENDINVENTORY spkt = new SMSG_SENDINVENTORY(lootlist.Count);
            foreach (KeyValuePair<byte, Rag2Item> c in lootlist.GetAllItems())
                spkt.AddItem(c.Value, c.Key);
            spkt.ActorID = owner.id;
            spkt.SessionId = character.id;
            character.client.Send((byte[])spkt);
        }

        protected internal void RequestLootLock()
        {
            this.islooting = false;
        }

        protected internal void ReleaseLootLock()
        {
            this.islooting = true;
        }

        #endregion Public Methods

        #region Public Static

        /// <summary>
        /// Create a collection of item drops
        /// </summary>
        /// <param name="mapobject"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static LootCollection Create(MapObject mapobject, Character character)
        {
            if (MapObject.IsPlayer(mapobject)) throw new SystemException("Cannot create loot for mapobjects");

            LootCollection collection = new LootCollection();
            PartySession party = character.sessionParty;

            if (party == null)
            {
                collection.LootLeader = character.id;
            }
            else if (party.LootSettings == 3 && party.ItemLeader.currentzone == mapobject.currentzone &&
                mapobject.currentzone.IsInSightRangeBySquare(mapobject.Position, party.ItemLeader.Position))
            {
                collection.LootLeader = party.ItemLeader.id;
            }
            else
            {
                collection.LootLeader = character.id;
            }

            // Generate loot from base mobs
            foreach (Rag2Item c in Singleton.Itemdrops.FindItemDropsById(mapobject.ModelId, character._DropRate))
            {
                collection.Lootlist.Add(c);
            }

            //Item drops from quest content
            foreach (Rag2Item item in QuestBase.UserQuestLoot(mapobject.ModelId, character))
            {
                collection.Lootlist.Add(item);
            }

            return collection;
        }

        #endregion Public Static

        #region Constructor / Deconstructor

        private LootCollection()
        {
            this.lootlist = new Rag2Collection();
        }

        ~LootCollection()
        {
            this.lootlist = null;
            this.isdisposed = true;
        }

        #endregion Constructor / Deconstructor

        #region IDisposable Members

        public void Dispose()
        {
            isdisposed = true;
            lootlist = null;
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            isdisposed = true;
            lootlist = null;
        }

        #endregion IDisposable Members
    }
}