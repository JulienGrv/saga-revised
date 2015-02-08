using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <remarks>
    /// This sends the comment message by a user when they
    /// request to see it.
    /// </remarks>
    /// <id>
    /// 1107
    /// </id>
    internal class SMSG_MARKETCOMMENT : RelayPacket
    {
        public SMSG_MARKETCOMMENT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1107;
            this.data = new byte[255];
        }

        public byte Reason
        {
            set
            {
                this.data[0] = value;
            }
        }

        public string Message
        {
            set
            {
                int length = Encoding.Unicode.GetBytes(value, 0, value.Length, this.data, 2);
                Array.Resize<byte>(ref this.data, length + 2);
                this.data[1] = (byte)length;
            }
        }
    }
}