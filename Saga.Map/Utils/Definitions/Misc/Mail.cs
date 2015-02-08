using Saga.PrimaryTypes;
using System;

namespace Saga.Map.Definitions.Misc
{
    public class Mail
    {
        public uint MailId;
        public string Sender;
        public string Topic;
        public string Message;
        public int Zeny;
        public DateTime Time = DateTime.Now;
        public byte IsRead = 0;
        public byte Valid = 0;
        public Rag2Item Attachment = null;
    }
}