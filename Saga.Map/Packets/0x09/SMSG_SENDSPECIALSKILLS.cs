using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Sends a list of special skills
    /// </summary>
    /// <remarks>
    /// This packet is sent when the user logs in for the first time
    /// (e.d no mapswitching). It's a list of all special skills the
    /// users has previously learned. After this packet all updates
    /// regarding the special skills are incremental.
    /// </remarks>
    /// <id>
    /// 0917
    /// </id>
    internal class SMSG_SENDSPECIALSKILLS : RelayPacket
    {
        public SMSG_SENDSPECIALSKILLS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0917;
            this.data = new byte[1];
        }

        public void Add(uint i)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 4);
            Array.Copy(BitConverter.GetBytes(i), 0, this.data, offset, 4);
            this.data[0]++;
        }
    }
}