using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client as a result of you have died and clicked
    /// on the desired warping spot. If the user doesn't click on either of the buttons
    /// withing 30 minutes, it get's warped to their save-location.
    /// </remarks>
    /// <id>
    /// 0308
    /// </id>
    internal class CMSG_SENDHOMEPOINT : RelayPacket
    {
        public CMSG_SENDHOMEPOINT()
        {
            this.data = new byte[1];
        }

        public byte Type
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SENDHOMEPOINT(byte[] p)
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

            CMSG_SENDHOMEPOINT pkt = new CMSG_SENDHOMEPOINT();
            pkt.data = new byte[p.Length - 14];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.cmd, 0, 2);
            Array.Copy(p, 12, pkt.id, 0, 2);
            Array.Copy(p, 14, pkt.data, 0, p.Length - 14);
            return pkt;
        }

        #endregion Conversions
    }
}