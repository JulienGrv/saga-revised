using System;
using Saga.Packets;
using Saga.Shared.Definitions;
using Saga.Tasks;
using Saga.PrimaryTypes;
using Saga.Map;
using Saga.Structures;

namespace Saga.Events
{
    public static partial class EventTable
    {

        public static void Rewards(uint CID)
        {
            Character character;
            if (LifeCycle.TryGetById(CID, out character))
            {
                SMSG_EVENTINFO2 spkt = new SMSG_EVENTINFO2();                
                foreach (EventItem rewardItem in Singleton.Database.FindEventItemList(character))
                    spkt.AddItem(rewardItem.EventId, rewardItem.ItemId, rewardItem.ItemCount);
                spkt.SessionId = character.id;
                character.client.Send((byte[])spkt);
            }
        }

    }
}
