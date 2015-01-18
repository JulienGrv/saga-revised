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
    /// This packet is sent by an player to indicate he/she 
    /// is agreeing to the content they see. Once both parties agree
    /// they'll be asked to confirm another time. 
    /// </remarks>
    /// <id>
    /// 0805
    /// </id>
    internal class CMSG_TRADECONTENTCONFIRM : RelayPacket
    {
        public CMSG_TRADECONTENTCONFIRM()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_TRADECONTENTCONFIRM(byte[] p)
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

            CMSG_TRADECONTENTCONFIRM pkt = new CMSG_TRADECONTENTCONFIRM();
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
