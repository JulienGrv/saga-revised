using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function sends over a list of visible additions. There is a maximum of 255 additons
    /// that can be visible by the client. Note this list is only meant for yourself.
    /// </remarks>
    /// <id>
    /// 051B
    /// </id>
    internal class SMSG_ADDITIONLIST : RelayPacket
    {
        public SMSG_ADDITIONLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051B;
            this.data = new byte[1];
        }

        public void Add(uint AdditionId, uint Time)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 8);
            Array.Copy(BitConverter.GetBytes(AdditionId), 0, this.data, offset, 4);
            Array.Copy(BitConverter.GetBytes(Time), 0, this.data, offset + 4, 4);
            this.data[0]++;
        }
    }
}