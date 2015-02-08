using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is depreciated since CB2. The client still preserves the packet however.
    /// due this reason this function is purely implacated for show.
    /// </remarks>
    /// <id>
    /// 0A01
    /// </id>
    internal class SMSG_ADDSHORTCUT : RelayPacket
    {
        public SMSG_ADDSHORTCUT()
        {
            this.data = new byte[10];
        }

        public byte ShortCutType
        {
            set { this.data[0] = value; }
        }

        public byte Slot
        {
            set { this.data[1] = value; }
        }

        public uint ShortcutId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }
    }
}