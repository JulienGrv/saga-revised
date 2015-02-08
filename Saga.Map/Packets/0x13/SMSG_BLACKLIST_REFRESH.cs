using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Refresh blacklist</summary>
    /// <remarks>
    /// This packet is sent by the server as a response
    /// on packet CMSG_BLACKLIST_REFRESH. The packet sents over a full copy
    /// of the blacklist with updated values.
    ///
    /// The maximum size of the blacklist is 10 characters.
    /// </remarks>
    /// <id>
    /// 1306
    /// </id>
    internal class SMSG_BLACKLIST_REFRESH : RelayPacket
    {
        public SMSG_BLACKLIST_REFRESH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1306;
            this.data = new byte[371];
        }

        public void Add(string name, byte reason)
        {
            int index = 1 + (this.data[0] * 37);
            UnicodeEncoding.Unicode.GetBytes(name, 0, Math.Min(name.Length, 16), this.data, index);
            this.data[index + 36] = reason;
            this.data[0]++;
        }
    }
}