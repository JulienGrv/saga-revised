using System;
using System.Collections.Generic;
using System.Text;
using Saga.Map.Definitions.Misc;
using Saga.PrimaryTypes;

namespace Saga.Map.Utils.Definitions.Misc
{
    public class MailItem
    {
        public string Recieptent;
        public string Topic;
        public string Content;
        public DateTime Timestamp = DateTime.Now;
        public uint Zeny;
        public Rag2Item item;
    }
}
