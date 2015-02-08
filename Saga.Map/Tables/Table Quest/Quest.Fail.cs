using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        public static void QuestFail(uint cid, uint questid)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                SMSG_QUESTFAIL spkt = new SMSG_QUESTFAIL();
                spkt.SessionId = value.id;
                spkt.QuestID = questid;
                value.client.Send((byte[])spkt);
                //value.QuestObjectives[questid] = null;
                Console.WriteLine("Test");
            }
        }
    }
}