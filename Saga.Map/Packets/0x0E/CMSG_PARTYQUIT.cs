using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This packet is sent by the user to indicate he/she wishes
    /// to quit the party.
    /// </remarks>
    /// <id>
    /// 0E04
    /// </id>
    internal class CMSG_PARTYQUIT : RelayPacket
    {
        public CMSG_PARTYQUIT()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_PARTYQUIT(byte[] p)
        {
            /*
            // Creates a new byte with the length of data
            // plus 4. The first size bytes are used like
            // [PacketSize][PacketId][PacketBody]
            //
            // Where Packet Size equals the length of the 
            // Packet body, Packet Identifier, Packet Size 
            // Container.
            */

            CMSG_PARTYQUIT pkt = new CMSG_PARTYQUIT();
            pkt.data = new byte[p.Length - 14];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.cmd, 0, 2);
            Array.Copy(p, 12, pkt.id, 0, 2);
            Array.Copy(p, 14, pkt.data, 0, p.Length - 14);
            return pkt;
        }

        #endregion
    }
}
