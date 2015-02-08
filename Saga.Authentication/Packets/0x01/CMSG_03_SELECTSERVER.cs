using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the player while he/she tries to login, and
    /// afterwards tries to select the server.
    /// </remarks>
    /// <id>
    /// 0304
    /// </id>
    public class CMSG_SELECTSERVER : RelayPacket
    {
        public CMSG_SELECTSERVER()
        {
            this.data = new byte[1];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SELECTSERVER(byte[] p)
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

            CMSG_SELECTSERVER pkt = new CMSG_SELECTSERVER();
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