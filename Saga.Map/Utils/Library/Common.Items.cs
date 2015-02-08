using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;

namespace Common
{
    public static class Items
    {
        public static bool GiveItem(Character character, uint item, byte count)
        {
            Rag2Item iventoryitem;
            if (Singleton.Item.TryGetItemWithCount(item, count, out iventoryitem))
            {
                int index = character.container.Add(iventoryitem);
                if (index > -1)
                {
                    character.container[index] = iventoryitem;
                    SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                    spkt.Container = 2;
                    spkt.UpdateReason = 0;
                    spkt.SetItem(iventoryitem, index);
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                    QuestBase.UserObtainedItem(item, character);
                    return true;
                }
            }

            return false;
        }
    }
}