using Saga.Enumarations;
using Saga.Managers;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Occurs when a user speaks in general to the given channel
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SENDCHAT(CMSG_SENDCHAT cpkt)
        {
            //Check if gm level is set prior to on checking if it's a gm command
            if (this.character.chatmute > 0)
            {
                CommonFunctions.Broadcast(this.character, this.character, "You've been muted");
            }
            else if (this.character.GmLevel >= 0 && ConsoleCommands.IsGMCommand(cpkt.Message))
            {
                ConsoleCommands.ParseGMCommand(cpkt.Message, this.character);
            }
            else
            {
                SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
                spkt.Message = cpkt.Message;
                spkt.Name = this.character.Name;
                spkt.MessageType = cpkt.MessageType;
                spkt.SessionId = this.character.id;

                switch (spkt.MessageType)
                {
                    case SMSG_SENDCHAT.MESSAGE_TYPE.CHANEL:
                        foreach (Character characterTarget in LifeCycle.Characters)
                        {
                            if (characterTarget.client.isloaded == false) continue;
                            spkt.SessionId = characterTarget.id;
                            characterTarget.client.Send((byte[])spkt);
                        }
                        break;

                    case SMSG_SENDCHAT.MESSAGE_TYPE.NORMAL:
                        foreach (MapObject myObject in this.character.currentzone.GetCharactersInSightRange(this.character))
                        {
                            Character characterTarget = (Character)myObject;
                            if (characterTarget.client.isloaded == false) continue;
                            spkt.SessionId = characterTarget.id;
                            characterTarget.client.Send((byte[])spkt);
                        }
                        break;

                    case SMSG_SENDCHAT.MESSAGE_TYPE.PARTY:
                        if (this.character.sessionParty != null)
                        {
                            foreach (Character myObject in this.character.sessionParty.GetCharacters())
                            {
                                spkt.SessionId = myObject.id;
                                myObject.client.Send((byte[])spkt);
                            }
                        }
                        else
                        {
                            spkt.MessageType2 = 0xFF;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }
                        break;

                    case SMSG_SENDCHAT.MESSAGE_TYPE.YELL:
                        foreach (MapObject myObject in this.character.currentzone.Regiontree.SearchActors(this.character, SearchFlags.Characters))
                        {
                            Character characterTarget = (Character)myObject;
                            if (characterTarget.client.isloaded == false) continue;
                            spkt.SessionId = characterTarget.id;
                            characterTarget.client.Send((byte[])spkt);
                        }
                        break;

                    default:
                        Trace.TraceError("Message type not found {0}", cpkt.MessageType);
                        spkt.MessageType2 = 0xFF;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when a player tries to pm a other player on the server.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SENDWHISPER(CMSG_SENDWHISPER cpkt)
        {
            //HELPER VARIABLES
            Character target;
            string user = cpkt.Name.ToUpperInvariant();
            string user2 = this.character.Name.ToUpperInvariant();
            byte result = 0;

            Predicate<KeyValuePair<string, byte>> BlacklistOwn = delegate(KeyValuePair<string, byte> pair)
            {
                return pair.Key == user;
            };

            Predicate<KeyValuePair<string, byte>> BlacklistFoo = delegate(KeyValuePair<string, byte> pair)
            {
                return pair.Key == user2;
            };

            //CHECK IF USER IS ONLINE
            if (LifeCycle.TryGetByName(user, out target))
            {
                if (target._blacklist.FindIndex(BlacklistFoo) > -1)
                    result = 2;
                else if (this.character._blacklist.FindIndex(BlacklistOwn) > -1)
                    result = 3;
            }
            else
            {
                result = 1;
            }

            //STRUCTURIZE PACKETS
            if (result > 0)
            {
                SMSG_WHISPERERROR spkt = new SMSG_WHISPERERROR();
                spkt.SessionId = this.character.id;
                spkt.Result = result;
                this.Send((byte[])spkt);
            }
            else
            {
                SMSG_SENDWHISPER spkt = new SMSG_SENDWHISPER();
                spkt.SessionId = this.character.id;
                spkt.Name = cpkt.Name;
                spkt.Message = cpkt.Message;
                spkt.Result = 1;
                this.Send((byte[])spkt);

                SMSG_SENDWHISPER spkt2 = new SMSG_SENDWHISPER();
                spkt2.SessionId = target.id;
                spkt2.Name = this.character.Name;
                spkt2.Message = cpkt.Message;
                spkt2.Result = 2;
                target.client.Send((byte[])spkt2);
            }
        }
    }
}