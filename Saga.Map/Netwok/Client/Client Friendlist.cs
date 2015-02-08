using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Registers a new friend to your friendlist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_FRIENDLIST_REGISTER(CMSG_FRIENDLIST_REGISTER cpkt)
        {
            //HELPER VARIABLES
            byte result = 0;
            byte clvl = 0;
            byte jlvl = 0;
            byte map = 0;
            byte job = 0;

            string nname = cpkt.Name;
            string name = cpkt.Name.ToUpperInvariant();

            //CHECK IF YOU CHOOSE YOURSELF
            if (character.Name.ToUpperInvariant() == name)
            {
                result = 4;
            }
            //CHECKS IF THE PLAYER IS ALREADY ADDED
            else if (character._friendlist.Contains(nname))
            {
                result = 3;
            }
            //CHECKS IF YOU DON'T OVERLAP YOUR MAXIMUM BOUNDS
            else if (character._friendlist.Count >= 50)
            {
                result = 2;
            }
            //VERIFY IF THE CHARACTER EXISTS
            else if (!Singleton.Database.VerifyNameExists(name))
            {
                result = 1;
            }
            //EVERYTHING IS OKAY
            else
            {
                Character target;

                Singleton.Database.InsertAsFriend(character.ModelId, nname);
                this.character._friendlist.Add(nname);
                if (LifeCycle.TryGetByName(name, out target))
                {
                    clvl = target._level;
                    jlvl = target.jlvl;
                    map = target.map;
                    job = target.job;
                }
            }

            //STRUCTURIZE PACKET
            SMSG_FRIENDLIST_REGISTER spkt = new SMSG_FRIENDLIST_REGISTER();
            spkt.Name = nname;
            spkt.Clvl = clvl;
            spkt.Jlvl = jlvl;
            spkt.Job = job;
            spkt.Map = map;
            spkt.SessionId = cpkt.SessionId;
            spkt.Reason = result;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Unregisters a friend from the friend list.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_FRIENDLIST_UNREGISTER(CMSG_FRIENDLIST_UNREGISTER cpkt)
        {
            //HELPER VARIABLES
            byte result = 0;
            string nname = cpkt.Name;
            string name = cpkt.Name.ToUpperInvariant();

            //CHECK IF A PLAYER IS SELECTED
            if (name.Length == 0)
            {
                //result = 6;
                return;
            }
            //CHECKS IF THE PLAYER EXISTS
            else if (!character._friendlist.Remove(nname))
            {
                result = 5;
            }
            //EVERYTHING IS OKAY
            else
            {
                Singleton.Database.DeleteFriend(character.ModelId, nname);
            }

            //STRUCTURIZE PACKET
            SMSG_FRIENDLIST_UNREGISTER spkt = new SMSG_FRIENDLIST_UNREGISTER();
            spkt.Name = nname;
            spkt.SessionId = cpkt.SessionId;
            spkt.Reason = result;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Refreshes the friendlist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_FRIENDLIST_REFRESH(CMSG_FRIENDLIST_REFRESH cpkt)
        {
            //HELPER VARIABLES
            Character character = this.character;

            //GENERATE A NEW FRIENDLIST PACKET
            SMSG_FRIENDLIST_REFRESH spkt = new SMSG_FRIENDLIST_REFRESH();
            spkt.SessionId = cpkt.SessionId;
            foreach (string s in character._friendlist)
            {
                Character target;
                if (LifeCycle.TryGetByName(s, out target))
                {
                    spkt.Add(s, target.job, target._level, target.jlvl, target.map);
                }
                else
                {
                    spkt.Add(s, 0, 0, 0, 0);
                }
            }
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Registers a new person to your blacklist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_BLACKLIST_REGISTER(CMSG_BLACKLIST_REGISTER cpkt)
        {
            //HELPER VARIABLES
            byte result = 0;
            byte reason = cpkt.Reason;
            string nname = cpkt.Name;
            string name = cpkt.Name.ToUpperInvariant();

            Predicate<KeyValuePair<string, byte>> searchQuery = delegate(KeyValuePair<string, byte> pair)
            {
                return pair.Key.ToUpperInvariant() == name;
            };

            //CHECK IF YOU CHOOSE YOURSELF
            if (character.Name.ToUpperInvariant() == name)
            {
                result = 4;
            }
            //CHECKS IF THE PLAYER IS ALREADY ADDED
            else if (character._blacklist.FindIndex(searchQuery) > -1)
            {
                result = 3;
            }
            //CHECKS IF YOU DON'T OVERLAP YOUR MAXIMUM BOUNDS
            else if (character._blacklist.Count >= 10)
            {
                result = 2;
            }
            //VERIFY IF THE CHARACTER EXISTS
            else if (!Singleton.Database.VerifyNameExists(name))
            {
                result = 1;
            }
            //EVERYTHING IS OKAY
            else
            {
                Singleton.Database.InsertAsBlacklist(character.ModelId, nname, reason);
                this.character._blacklist.Add(new KeyValuePair<string, byte>(nname, reason));
                //If the character is on friendslist then remove them.
                if (this.character._friendlist.Contains(nname)) this.character._friendlist.Remove(nname);
            }

            //STRUCTURIZE PACKET
            SMSG_BLACKLIST_REGISTER spkt = new SMSG_BLACKLIST_REGISTER();
            spkt.Name = nname;
            spkt.SessionId = cpkt.SessionId;
            spkt.Reason = cpkt.Reason;
            spkt.Result = result;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Unregisters a existing person from your blacklist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_BLACKLIST_UNREGISTER(CMSG_BLACKLIST_UNREGISTER cpkt)
        {
            //HELPER VARIABLES
            byte result = 0;
            string nname = cpkt.Name;
            string name = cpkt.Name.ToUpperInvariant();
            Character character = this.character;

            Predicate<KeyValuePair<string, byte>> searchQuery = delegate(KeyValuePair<string, byte> pair)
            {
                return pair.Key.ToUpperInvariant() == name;
            };

            //CHECK IF A PLAYER IS SELECTED
            if (name.Length == 0)
            {
                //result = 6;
                return;
            }
            //CHECKS IF THE PLAYER EXISTS
            else if (character._blacklist.RemoveAll(searchQuery) <= 0)
            {
                result = 5;
            }
            //EVERYTHING IS OKAY
            else
            {
                Singleton.Database.DeleteBlacklist(this.character.ModelId, nname);
            }

            //STRUCTURIZE PACKET
            SMSG_BLACKLIST_UNREGISTER spkt = new SMSG_BLACKLIST_UNREGISTER();
            spkt.Name = nname;
            spkt.SessionId = cpkt.SessionId;
            spkt.Reason = result;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Refreshes the blacklist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_BLACKLIST_REFRESH(CMSG_BLACKLIST_REFRESH cpkt)
        {
            //HELPER VARIABLES
            GC.KeepAlive(this.character);
            SMSG_BLACKLIST_REFRESH spkt = new SMSG_BLACKLIST_REFRESH();
            spkt.SessionId = cpkt.SessionId;
            foreach (KeyValuePair<string, byte> s in character._blacklist)
                spkt.Add(s.Key, s.Value);
            this.Send((byte[])spkt);
        }
    }
}