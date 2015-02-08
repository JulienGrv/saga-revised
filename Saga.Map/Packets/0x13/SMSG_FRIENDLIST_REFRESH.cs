using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Refresh friendlist</summary>
    /// <remarks>
    /// This packet is sent by the server as a response
    /// on packet CMSG_FRIENDLIST_REFRESH. The packet sents over a full copy
    /// of the friendlist with updated values.
    ///
    /// The maximum size of the blacklist is 10 characters.
    /// </remarks>
    /// <id>
    /// 1303
    /// </id>
    internal class SMSG_FRIENDLIST_REFRESH : RelayPacket
    {
        public SMSG_FRIENDLIST_REFRESH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1303;
            this.data = new byte[801];
        }

        public void Add(string name, byte job, byte clvl, byte jlvl, byte map)
        {
            int index = 1 + (this.data[0] * 40);
            UnicodeEncoding.Unicode.GetBytes(name, 0, Math.Min(name.Length, 16), this.data, index);
            this.data[index + 36] = job;
            this.data[index + 37] = clvl;
            this.data[index + 38] = jlvl;
            this.data[index + 39] = map;
            this.data[0]++;
        }
    }
}