using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to invite a person by it's name.
    /// </remarks>
    /// <id>
    /// 0D01
    /// </id>
    internal class SMSG_PARTYINVITATION : RelayPacket
    {
        public SMSG_PARTYINVITATION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D01;
            this.data = new byte[34];
        }

        public string Name
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(16, value.Length), this.data, 0); }
        }
    }
}