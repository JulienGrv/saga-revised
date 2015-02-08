using Saga.Enumarations;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Occurs when the party requests is made
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PARTYREQUEST(CMSG_PARTYINVITE_LOCAL cpkt)
        {
            //HELPER VARIABLES
            Character target;
            PartySession partysession = (this.character.sessionParty == null) ? new PartySession(this.character) : this.character.sessionParty;

            //CHECK IF IT DOESN'T OVERLAP THE PARTY BOUNDARIES
            if (partysession != null && partysession.Count >= 6)
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = (byte)PartyErrors.MAX_MEMBER;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //CHECK IF THE PLAYER CAN INVITE NEW MEMBERS
            else if (partysession != null && partysession.PartyLeader != this.character)
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //VERIFY IF PLAYER EXISTS
            else if (LifeCycle.TryGetById(cpkt.ActorId, out target))
            {
                //CHECK IF A PLAYER IS ALREADY IN PARTY
                if (target.sessionParty != null)
                {
                    SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                    spkt.Result = (byte)PartyErrors.ALREADY_IN_PARTY;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                //FORWARD INVITATION
                else if (partysession.PartyLeader == this.character)
                {
                    SMSG_PARTYINVITATION spkt = new SMSG_PARTYINVITATION();
                    spkt.SessionId = target.id;
                    spkt.Name = this.character.Name;
                    target.client.Send((byte[])spkt);

                    //Cross reference session, Tag for knowing who was the invitee
                    target.sessionParty = partysession;
                    target.Tag = this.character.id;
                    this.character.sessionParty = partysession;
                    this.character.Tag = target.id;
                }
                else
                {
                    Trace.TraceInformation("Not a party leader");
                }
            }
            //PLAYER DOES NOT EXISTS
            else
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when the party requests is made
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PARTYREQUEST(CMSG_PARTYINVITE cpkt)
        {
            //HELPER VARIABLES
            string name = cpkt.Name;
            Character target;
            PartySession partysession = (this.character.sessionParty == null) ? new PartySession(this.character) : this.character.sessionParty;

            //CHECK IF IT DOESN'T OVERLAP THE PARTY BOUNDARIES
            if (partysession != null && partysession.Count >= 6)
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = (byte)PartyErrors.MAX_MEMBER;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //CHECK IF THE PLAYER CAN INVITE NEW MEMBERS
            else if (partysession != null && partysession.PartyLeader != this.character)
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //VERIFY IF PLAYER EXISTS
            else if (LifeCycle.TryGetByName(name, out target))
            {
                //CHECK IF A PLAYER IS ALREADY IN PARTY
                if (target.sessionParty != null)
                {
                    SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                    spkt.Result = (byte)PartyErrors.ALREADY_IN_PARTY;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                //FORWARD INVITATION
                else if (partysession.PartyLeader == this.character)
                {
                    SMSG_PARTYINVITATION spkt = new SMSG_PARTYINVITATION();
                    spkt.SessionId = target.id;
                    spkt.Name = this.character.Name;
                    target.client.Send((byte[])spkt);

                    //Cross reference session, Tag for knowing who was the invitee
                    target.sessionParty = partysession;
                    target.Tag = this.character.id;
                    this.character.sessionParty = partysession;
                    this.character.Tag = target.id;
                }
                else
                {
                    Trace.TraceInformation("Not a party leader");
                }
            }
            //PLAYER DOES NOT EXISTS
            else
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = 2;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when the party invitation gets accepted
        /// </summary>
        private void CM_PARTYINVITATIONACCEPT(CMSG_PARTYINVITATIONACCEPT cpkt)
        {
            //FORWARD A OKAY MESSAGE TO THE SENDER AND RECIEVER OF THE INIVATION
            PartySession party = this.character.sessionParty;
            if (party != null)
            {
                if (cpkt.Status == 1)
                {
                    //Has accepted the party invitation
                    {
                        SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                        spkt.Result = 0;
                        spkt.SessionId = party.PartyLeader.id;
                        party.PartyLeader.client.Send((byte[])spkt);
                    }
                    //Send over a new party
                    if (this.character.sessionParty.Count == 1)
                    {
                        SMSG_PARTYMEMBERINFO spkt = new SMSG_PARTYMEMBERINFO();
                        spkt.Leader = party.PartyLeader.id;
                        spkt.LeaderIndex = 1;
                        spkt.Setting1 = this.character.sessionParty.LootSettings;
                        spkt.Setting2 = this.character.sessionParty.ExpSettings;
                        spkt.Setting3 = 0;
                        if (this.character.sessionParty.ItemLeader != null)
                            spkt.Setting4 = this.character.sessionParty.ItemLeader.id;
                        spkt.AddMemberInfo(party.GetCharacters());
                        spkt.SessionId = party.PartyLeader.id;
                        spkt.Result = 1;
                        party.PartyLeader.client.Send((byte[])spkt);
                    }

                    //Forwards new party member information to all existing players
                    this.character.sessionParty.Add(this.character);
                    int index = party.IndexOf(this.character) + 1;

                    //Process character adding
                    foreach (Character target in party.GetCharacters())
                    {
                        //Send a new character to other party members
                        if (target.id != character.id)
                        {
                            SMSG_PARTYNEWMEMBER spkt2 = new SMSG_PARTYNEWMEMBER();
                            spkt2.Index = 1;
                            spkt2.ActorId = this.character.id;
                            spkt2.Unknown = 1;
                            spkt2.Name = this.character.Name;
                            spkt2.SessionId = target.id;
                            target.client.Send((byte[])spkt2);
                        }
                        //Send a complete list to yourself
                        else
                        {
                            SMSG_PARTYMEMBERINFO spkt7 = new SMSG_PARTYMEMBERINFO();
                            spkt7.Leader = party.PartyLeader.id;
                            spkt7.LeaderIndex = 1;
                            spkt7.Setting1 = this.character.sessionParty.LootSettings;
                            spkt7.Setting2 = this.character.sessionParty.ExpSettings;
                            spkt7.Setting3 = 0;
                            if (this.character.sessionParty.ItemLeader != null)
                                spkt7.Setting4 = this.character.sessionParty.ItemLeader.id;
                            spkt7.Result = 1;
                            spkt7.SessionId = this.character.id;
                            spkt7.AddMemberInfo(party.GetCharacters());
                            this.Send((byte[])spkt7);
                        }
                    }

                    //Forwards new party member location and HP/SP
                    //informations
                    foreach (Character target in party.GetCharacters())
                    {
                        if (target.id == character.id) continue;

                        //Send over target to myself
                        SMSG_PARTYUNKNOWN spkt4 = new SMSG_PARTYUNKNOWN();
                        spkt4.ActorID = target.id;
                        spkt4.Unsure = 1;
                        spkt4.Unknown = (byte)(this.character.map + 0x65);
                        spkt4.Race = 0;
                        spkt4.Map = target.map;
                        spkt4.HP = target.HP;
                        spkt4.HPmax = target.HPMAX;
                        spkt4.SP = target.SP;
                        spkt4.SPmax = target.SPMAX;
                        spkt4.LP = target._status.CurrentLp;
                        spkt4.CharLvl = target._level;
                        spkt4.Job = target.job;
                        spkt4.JobLvl = target.jlvl;
                        spkt4.SessionId = this.character.id;
                        this.Send((byte[])spkt4);

                        //Send my self to target
                        SMSG_PARTYUNKNOWN spkt3 = new SMSG_PARTYUNKNOWN();
                        spkt3.ActorID = this.character.id;
                        spkt3.Unsure = 1;
                        spkt3.Unknown = (byte)(this.character.map + 0x65);
                        spkt3.Race = 0;
                        spkt3.Map = this.character.map;
                        spkt3.HP = this.character.HP;
                        spkt3.HPmax = this.character.HPMAX;
                        spkt3.SP = this.character.SP;
                        spkt3.SPmax = this.character.SPMAX;
                        spkt3.LP = this.character._status.CurrentLp;
                        spkt3.CharLvl = this.character._level;
                        spkt3.Job = this.character.job;
                        spkt3.JobLvl = this.character.jlvl;
                        spkt3.SessionId = target.id;
                        target.client.Send((byte[])spkt3);

                        //Process positon update
                        if (target.map == this.character.map)
                        {
                            SMSG_PARTYMEMBERLOCATION spkt6 = new SMSG_PARTYMEMBERLOCATION();
                            spkt6.Index = 1;
                            spkt6.ActorId = target.id;
                            spkt6.X = target.Position.x;
                            spkt6.Y = target.Position.y;
                            spkt6.SessionId = this.character.id;
                            this.Send((byte[])spkt6);

                            SMSG_PARTYMEMBERLOCATION spkt5 = new SMSG_PARTYMEMBERLOCATION();
                            spkt5.Index = 1;
                            spkt5.ActorId = this.character.id;
                            spkt5.X = this.character.Position.x;
                            spkt5.Y = this.character.Position.y;
                            spkt5.SessionId = target.id;
                            target.client.Send((byte[])spkt5);
                        }
                    }
                }
                else
                {
                    Character PartyLeader = this.character.sessionParty.PartyLeader;
                    SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                    spkt.Result = (byte)PartyErrors.DENIED;
                    spkt.SessionId = PartyLeader.id;
                    PartyLeader.client.Send((byte[])spkt);

                    try
                    {
                        uint TargetActorId = (uint)this.character.Tag;
                        Character TargetActor = null;
                        if (LifeCycle.TryGetById(TargetActorId, out TargetActor))
                        {
                            TargetActor.Tag = null;
                            if (TargetActor.sessionParty._Characters.Count <= 1)
                                TargetActor.sessionParty = null;
                        }
                    }
                    catch (InvalidCastException)
                    {
                        Trace.TraceWarning("Cannot retrieve actor information");
                    }

                    if (this.character.sessionParty.Count <= 1)
                        this.character.sessionParty = null;
                    this.character.Tag = null;
                }
            }
            //FORWARD A DENIAL MESSAGE TO THE RECIEVER OF THE INVITATION
            else
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = (byte)PartyErrors.DENIED;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
                this.character.sessionParty = null;
            }
        }

        /// <summary>
        /// Occurs when an player cancles the party.
        /// </summary>
        ///

        private void CM_PARTYINVITATIONCANCELED(CMSG_PARTYINVITECANCEL cpkt)
        {
            //FORWARD A OKAY MESSAGE TO THE SENDER AND RECIEVER OF THE INIVATION
            if (this.character.sessionParty != null)
            {
                {
                    SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                    spkt.Result = 1;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                {
                    SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                    spkt.Result = 1;
                    spkt.SessionId = this.character.sessionParty.PartyLeader.id;
                    this.character.sessionParty.PartyLeader.client.Send((byte[])spkt);

                    Character lootleader = this.character.sessionParty.PartyLeader;
                    if (this.character.sessionParty.Count <= 1)
                        lootleader.sessionParty = null;
                    this.character.sessionParty = null;
                }
            }
            //FORWARD A DENIAL MESSAGE TO THE RECIEVER OF THE INVITATION
            else
            {
                SMSG_PARTYINVITATIONRESULT spkt = new SMSG_PARTYINVITATIONRESULT();
                spkt.Result = 1;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
                this.character.sessionParty = null;
            }
        }

        /// <summary>
        /// Occurs when an players tries to quite for misc reasons.
        /// They might log off or they might just want to stop the party
        /// </summary>
        private void CM_PARTYQUIT(CMSG_PARTYQUIT cpkt)
        {
            if (this.character.sessionParty == null) return;

            //Pregenerate quite message
            SMSG_PARTYQUIT spkt = new SMSG_PARTYQUIT();
            spkt.ActorId = this.character.id;
            spkt.Index = (byte)(this.character.sessionParty.IndexOf(this.character) + 1);
            spkt.Reason = 1;

            //Forwards to all members that i quite
            foreach (Character c in this.character.sessionParty.GetCharacters())
            {
                if (c.client.isloaded == false) continue;
                spkt.SessionId = c.id;
                c.client.Send((byte[])spkt);
            }

            //Removes the current character
            this.character.sessionParty.Remove
            (
                this.character
            );

            //Dismiss the party if everybody left
            if (this.character.sessionParty.Count == 1)
            {
                Character lastchar = this.character.sessionParty._Characters[0];
                SMSG_PARTYDISMISSED spkt2 = new SMSG_PARTYDISMISSED();
                spkt2.SessionId = lastchar.id;
                lastchar.client.Send((byte[])spkt2);
                lastchar.sessionParty = null;
            }

            //Clears the party object
            this.character.sessionParty = null;
        }

        /// <summary>
        /// Occurs when kicking a players from the party
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PARTYKICK(CMSG_PARTYKICK cpkt)
        {
            if (this.character.sessionParty == null) return;

            //Check if party is party leader
            if (this.character.sessionParty.PartyLeader != this.character)
            {
                Trace.TraceInformation("Non party leader attempt to set settings");
                return;
            }
            //Removes the current character
            else if (cpkt.Index > this.character.sessionParty._Characters.Count)
            {
                Trace.TraceInformation("Party out of range");
                return;
            }

            uint actorid = this.character.sessionParty[cpkt.Index].id;
            this.character.sessionParty._Characters.RemoveAt(cpkt.Index);

            //Forwards to all members that i quite
            foreach (Character c in this.character.sessionParty.GetCharacters())
            {
                //Pregenerate quite message
                SMSG_PARTYQUIT spkt = new SMSG_PARTYQUIT();
                spkt.ActorId = actorid;
                spkt.Index = 1;
                spkt.Reason = 1;
                spkt.SessionId = c.id;
                c.client.Send((byte[])spkt);
            }

            //Dismiss the party if everybody left
            if (this.character.sessionParty.Count == 1)
            {
                Character lastchar = this.character.sessionParty._Characters[0];
                SMSG_PARTYDISMISSED spkt2 = new SMSG_PARTYDISMISSED();
                spkt2.SessionId = lastchar.id;
                lastchar.client.Send((byte[])spkt2);
            }
        }

        /// <summary>
        /// Occurs when changing the party settings
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PARTYMODE(CMSG_PARTYMODE cpkt)
        {
            if (this.character.sessionParty == null) return;
            if (this.character.sessionParty.PartyLeader != this.character)
            {
                Trace.TraceInformation("Non party leader attempt to set settings");
                return;
            }

            for (int i = 0; i < this.character.sessionParty.Count; i++)
            {
                Character partycharacter = this.character.sessionParty._Characters[i];
                SMSG_PARTYMODE spkt = new SMSG_PARTYMODE();
                spkt.SessionId = partycharacter.id;
                spkt.LootShare = cpkt.LootShare;
                spkt.ExpShare = cpkt.ExpShare;
                spkt.Unknown = cpkt.Unknown;
                spkt.LootMaster = cpkt.LootMaster;
                partycharacter.client.Send((byte[])spkt);
            }

            this.character.sessionParty.LootSettings = cpkt.LootShare;
            this.character.sessionParty.ExpSettings = cpkt.ExpShare;
            this.character.sessionParty._LootLeader =
                this.character.sessionParty.GetMemberById(cpkt.LootMaster);
        }

        /// <summary>
        /// Occurs after setting the load leader
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PARTYSETLEADER(CMSG_PARTYSETLEADER cpkt)
        {
            if (this.character.sessionParty.PartyLeader != this.character)
            {
                Trace.TraceInformation("Non party leader attempt to set party leader");
                return;
            }

            int result = 0;
            uint id = 0;
            string name = cpkt.Name;

            if (this.character.sessionParty == null) return;
            for (int i = 0; i < this.character.sessionParty._Characters.Count; i++)
            {
                Character partycharacter = this.character.sessionParty._Characters[i];
                if (partycharacter.Name == name)
                {
                    result = i;
                    id = partycharacter.id;
                }
            }

            SMSG_PARTYSETLEADER spkt = new SMSG_PARTYSETLEADER();
            spkt.Index = 1;
            spkt.ActorId = id;
            foreach (Character partycharacter in this.character.sessionParty.GetCharacters())
            {
                spkt.SessionId = partycharacter.id;
                partycharacter.client.Send((byte[])spkt);
            }
        }
    }
}