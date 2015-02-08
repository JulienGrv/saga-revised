using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sents a list of navigation points of activated
    /// npc's for the current map. If the list needs to be modified
    /// you need to clear the list before.
    /// </remarks>
    /// <id>
    /// 070B
    /// </id>
    internal class SMSG_SENDNAVIGATIONPOINT : RelayPacket
    {
        public SMSG_SENDNAVIGATIONPOINT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x070B;
            this.data = new byte[5];
            this.data[0] = 0;
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public void AddPosition(uint npctype, float x, float y, float z)
        {
            //INCREMENT THE COUNTER
            this.data[0]++;
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, this.data.Length + 16);

            //ADD NEW WAYPOINT
            Array.Copy(BitConverter.GetBytes(npctype), 0, this.data, offset, 4);
            FloatToArray(x / 1000, this.data, offset + 4);
            FloatToArray(y / 1000, this.data, offset + 8);
            FloatToArray(z / 1000, this.data, offset + 12);
        }
    }
}