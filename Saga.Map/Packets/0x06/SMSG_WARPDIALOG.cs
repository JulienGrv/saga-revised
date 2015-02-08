using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet shows a list of warp locations where you can choose
    /// where to warp too.
    /// </remarks>
    /// <id>
    /// 0620
    /// </id>
    internal class SMSG_WARPDIALOG : RelayPacket
    {
        public SMSG_WARPDIALOG()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0620;
            this.data = new byte[5];
            this.data[4] = 0;
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public void AddItem(ushort id, uint price)
        {
            int position = this.data.Length;
            Array.Resize<byte>(ref this.data, this.data.Length + 337);
            Array.Copy(BitConverter.GetBytes(id), 0, this.data, position, 2);
            Array.Copy(BitConverter.GetBytes(price), 0, this.data, position + 2, 4);
            this.data[4]++;
        }
    }
}