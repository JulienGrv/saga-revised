using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This show your characters rebuy list.
    /// </remarks>
    /// <id>
    /// 060D
    /// </id>
    internal class SMSG_REBUYLIST : RelayPacket
    {
        public SMSG_REBUYLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x060D;
            this.data = new byte[1];
        }

        public byte Count
        {
            set
            {
                this.data[0] = value;
                Array.Resize<byte>(ref this.data, 1 + value * 67);
            }
        }

        public void Add(uint itemID, byte count)
        {
            int index = this.data.Length - 67;
            Array.Copy(BitConverter.GetBytes(itemID), 0, this.data, index, 4);

            this.data[index + 4] = 0x3A;
            this.data[index + 5] = 0x95;
            this.data[index + 6] = 0x9F;
            this.data[index + 7] = 0x47;

            this.data[index + 8] = 0xBA;
            this.data[index + 9] = 0x16;
            this.data[index + 10] = 0x0C;
            this.data[index + 11] = 0x01;

            this.data[index + 53] = count;
            this.data[index + 66] = this.data[0];
        }
    }
}