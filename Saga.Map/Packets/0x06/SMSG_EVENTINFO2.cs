using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet shows you a list of items.
    /// Due lack of information how this works is still unknown
    /// </remarks>
    /// <id>
    /// 061E
    /// </id>
    internal class SMSG_EVENTINFO2 : RelayPacket
    {
        public SMSG_EVENTINFO2()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061E;
            this.data = new byte[2];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public void AddItem(uint unknown, uint itemid, byte count)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 9);
            Array.Copy(BitConverter.GetBytes(unknown), 0, this.data, offset, 4);
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, offset + 4, 4);
            this.data[offset + 8] = count;
            this.data[1]++;
        }
    }
}