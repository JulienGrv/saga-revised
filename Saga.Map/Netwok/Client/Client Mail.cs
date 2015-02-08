using Saga.Enumarations;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;

#pragma warning disable 0436

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Occurs when requesting to refresh the inbox list with mail items.
        /// </summary>
        private void CM_REQUESTINBOXMAILLIST(CMSG_REQUESTINOX cpkt)
        {
            SMSG_MAILLIST spkt = new SMSG_MAILLIST();
            spkt.SessionId = this.character.id;
            spkt.SourceActor = this.character.id;
            foreach (Mail c in Singleton.Database.GetInboxMail(this.character))
                spkt.AddMail(c);
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Occurs when requesting to refresh the outbox list with mail items.
        /// </summary>
        private void CM_REQUESTOUTBOXMAILLIST(CMSG_REQUESTOUTBOX cpkt)
        {
            SMSG_MAILLISTOUTBOX spkt = new SMSG_MAILLISTOUTBOX();
            spkt.SessionId = this.character.id;
            spkt.SourceActor = this.character.id;
            foreach (Mail c in Singleton.Database.GetOutboxMail(this.character))
                spkt.AddMail(c.MailId, c.Sender, c.Topic, 1, c.Time, 0, 0);
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Sends a new mail message
        /// </summary>
        private void CM_NEWMAILITEM(CMSG_SENDMAIL cpkt)
        {
            //HELPER VARIABLES
            byte result = 1;
            uint req_zeny = 0;
            Rag2Item item = null;
            MailItem mailmessage = new MailItem();
            mailmessage.Content = cpkt.Content;
            mailmessage.Recieptent = cpkt.Name;
            mailmessage.Topic = cpkt.Topic;
            mailmessage.item = item;

            if ((cpkt.HasItem & 2) == 2)
            {
                item = this.character.container[cpkt.Slot];
                if (item != null)
                {
                    req_zeny = 10;
                    mailmessage.item = item.Clone(cpkt.StackCount);
                }
            }
            if ((cpkt.HasItem & 1) == 1)
            {
                req_zeny = 10 + cpkt.Zeny;
                mailmessage.Zeny = cpkt.Zeny;
            }

            try
            {
                //RECIEVER DOES NOT EXISTS
                if (!Singleton.Database.VerifyNameExists(mailmessage.Recieptent))
                {
                    result = 2;
                }
                //NOT ENOUGH MONEY
                else if (this.character.ZENY < req_zeny)
                {
                    result = 3;
                }
                //CHECK ITEM INVENTORY
                else if (item != null && cpkt.StackCount > item.count)
                {
                    result = 5;
                }
                //CHECK IF OWNER OUTBOX IF FULL
                else if (Singleton.Database.GetInboxMailCount(this.character.Name) == 20)
                {
                    result = 6;
                }
                //CHECK IF SENDER INBOX IS FULL
                else if (Singleton.Database.GetInboxMailCount(mailmessage.Recieptent) == 20)
                {
                    result = 7;
                }
                //DATABASE ERROR
                else if (!Singleton.Database.InsertNewMailItem(this.character, mailmessage))
                {
                    result = 1;
                }
                //EVERYTHING IS OKAY
                else
                {
                    if (cpkt.HasItem > 0)
                    {
                        this.character.ZENY -= req_zeny;
                        CommonFunctions.UpdateZeny(this.character);
                    }

                    //UPDATE ITEM COUNT AS FORM OF A ATTACHMENT
                    if ((cpkt.HasItem & 2) == 2)
                    {
                        item.count -= cpkt.StackCount;
                        if (item.count > 0)
                        {
                            SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                            spkt.Amount = (byte)item.count;
                            spkt.UpdateReason = (byte)ItemUpdateReason.AttachmentReceived;
                            spkt.UpdateType = 4;
                            spkt.Container = 2;
                            spkt.SessionId = this.character.id;
                            spkt.Index = cpkt.Slot;
                            this.Send((byte[])spkt);
                        }
                        else
                        {
                            this.character.container.RemoveAt(cpkt.Slot);
                            SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                            spkt.Container = 2;
                            spkt.Index = cpkt.Slot;
                            spkt.UpdateReason = (byte)ItemUpdateReason.AttachmentReceived;
                            spkt.SessionId = this.character.id;
                            this.Send((byte[])spkt);
                        }
                    }

                    //EVERYTHING OKAY
                    result = 0;
                }
            }
            finally
            {
                SMSG_MAILSENDAWNSER spkt = new SMSG_MAILSENDAWNSER();
                spkt.Result = result;
                spkt.SessionId = cpkt.SessionId;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Retrieves a mail item by it's given mail id.
        /// </summary>
        /// <notes>
        /// Gamestring errors doesn't seem to be supported yet?
        /// Or perhaps in a different packet. The unknown field is most
        /// likely going to be the error field so i pretended it is the
        /// error field for now.
        /// </notes>
        private void CM_INBOXMAILITEM(CMSG_GETMAIL cpkt)
        {
            //TODO: REVERSE THE GAMESTRING ERRORS
            //HELPER VARIABLES

            byte result = 1;
            MailItem item = null;

            try
            {
                //GET THE MAIL FROM THE DB
                item = Singleton.Database.GetMailItemById(cpkt.MailId);
                Singleton.Database.MarkAsReadMailItem(cpkt.MailId);

                //MARKED AS OKAY
                result = 0;
            }
            finally
            {
                SMSG_MAILDATA spkt = new SMSG_MAILDATA();
                spkt.Unknown = result;
                spkt.SessionId = this.character.id;

                if (item != null)
                {
                    spkt.Sender = item.Recieptent;
                    spkt.Date = item.Timestamp;
                    spkt.Topic = item.Topic;
                    spkt.Content = item.Content;
                    spkt.Zeny = item.Zeny;
                    spkt.item = item.item;
                }

                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when retrieving item attachement from the id.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_RETRIEVEITEMATTACHMENT(CMSG_GETITEMATTACHMENT cpkt)
        {
            byte result = 0;
            try
            {
                Rag2Item item = Singleton.Database.GetItemAttachment(cpkt.MailId);

                //No attachment
                if (item == null)
                {
                    result = 1;
                }
                //Not the same item type
                else if (item.info.item != cpkt.ItemId)
                {
                    result = 1;
                }
                //Not enough space
                else if (this.character.container.Count == this.character.container.Capacity)
                {
                    result = 1;
                }
                //Update the database
                else if (Singleton.Database.UpdateItemAttachment(cpkt.MailId, null) == false)
                {
                    result = 1;
                }
                //Everything is okay
                else
                {
                    int index = this.character.container.Add(item);
                    SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                    spkt.Container = 2;
                    spkt.UpdateReason = (byte)ItemUpdateReason.AttachmentSent;
                    spkt.SessionId = this.character.id;
                    spkt.SetItem(item, index);
                    this.Send((byte[])spkt);
                }
            }
            finally
            {
                SMSG_MAILITEMAWNSER spkt = new SMSG_MAILITEMAWNSER();
                spkt.SessionId = cpkt.SessionId;
                spkt.Result = result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when retrieving the money of a mail
        /// </summary>
        /// <remarks>
        /// This gives you the zeny from your mail. However
        /// in the case that your new zeny overlaps the maximum
        /// value of a uint. (The data type used to send over a zeny).
        /// We'll simply not give you the money and send a fail
        /// message in return.
        /// </remarks>
        private void CM_RETRIEVEZENYATTACHMENT(CMSG_GETZENYATTACHMENT cpkt)
        {
            //HELPER VARIABLES
            byte result = 0;

            //TRY TO GET THE ZENY
            try
            {
                uint zeny = Singleton.Database.GetZenyAttachment(cpkt.MailId);

                //VERIFY CLIENT GIVEN VALUE IS CORRECT
                if (cpkt.Zeny > zeny)
                {
                    result = 1;
                }
                //EXCEEDED MAX ZENY VALUE
                else if ((((long)this.character.ZENY + (long)cpkt.Zeny)) > (long)uint.MaxValue)
                {
                    result = 3;
                }
                //EVERYTHING OKAY
                else
                {
                    Singleton.Database.UpdateZenyAttachment(cpkt.MailId, 0);
                    this.character.ZENY += cpkt.Zeny;
                    CommonFunctions.UpdateZeny(this.character);
                }

                SMSG_MAILZENYAWNSER spkt = new SMSG_MAILZENYAWNSER();
                spkt.SessionId = this.character.id;
                spkt.Result = result;
                this.Send((byte[])spkt);
            }
            catch (Exception)
            {
                SMSG_MAILZENYAWNSER spkt = new SMSG_MAILZENYAWNSER();
                spkt.SessionId = this.character.id;
                spkt.Result = result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when deleting a mail from the inbox
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MAILDELETE(CMSG_DELETEMAIL cpkt)
        {
            int result = 1;
            try
            {
                result = (Singleton.Database.DeleteMails(cpkt.MailId)) ? 0 : 1;
            }
            finally
            {
                SMSG_MAILDELETEAWNSER spkt = new SMSG_MAILDELETEAWNSER();
                spkt.SessionId = cpkt.SessionId;
                spkt.Result = (byte)result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when canceling the mail message
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MAILCANCEL(CMSG_MAILCANCEL cpkt)
        {
            int result = 1;
            try
            {
                result = (Singleton.Database.DeleteMailFromOutbox(cpkt.MailId)) ? 0 : 1;
            }
            finally
            {
                SMSG_MAILCANCELAWNSER spkt = new SMSG_MAILCANCELAWNSER();
                spkt.SessionId = cpkt.SessionId;
                spkt.Result = (byte)result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when deleting mail in outbox
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MAILCLEAR(CMSG_MAILCLEAR cpkt)
        {
            int result = 1;
            try
            {
                MailItem item = Singleton.Database.GetMailItemById(cpkt.MailId);
                if (item == null)
                {
                    result = 1;
                }
                else
                {
                    item.Topic = "Cleared";
                    item.Content = "Cleared";
                    item.Recieptent = this.character.Name;
                    item.Timestamp = DateTime.Now;

                    result = (Singleton.Database.DeleteMailFromOutbox(cpkt.MailId)) ? 0 : 1;
                    if (result == 0)
                        Singleton.Database.InsertNewMailItem(null, item);
                }
            }
            finally
            {
                SMSG_MAILCLEARAWNSER spkt = new SMSG_MAILCLEARAWNSER();
                spkt.SessionId = cpkt.SessionId;
                spkt.Result = (byte)result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when retrieving a outbox mailitem
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_OUTBOXMAILITEM(CMSG_GETMAILOUTBOX cpkt)
        {
            //TODO: REVERSE THE GAMESTRING ERRORS
            //HELPER VARIABLES

            byte result = 1;
            MailItem item = null;

            try
            {
                //GET THE MAIL FROM THE DB
                item = Singleton.Database.GetMailItemById(cpkt.MailId);

                //MARKED AS OKAY
                result = 0;
            }
            finally
            {
                SMSG_MAILOUTDATA spkt = new SMSG_MAILOUTDATA();
                spkt.Unknown = result;
                spkt.SessionId = this.character.id;

                if (item != null)
                {
                    spkt.Sender = item.Recieptent;
                    spkt.Date = item.Timestamp;
                    spkt.Topic = item.Topic;
                    spkt.Content = item.Content;
                    spkt.item = item.item;
                }

                this.Send((byte[])spkt);
            }
        }
    }
}

#pragma warning restore 0436