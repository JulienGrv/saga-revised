using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Tasks;

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