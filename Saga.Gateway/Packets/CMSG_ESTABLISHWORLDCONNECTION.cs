using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as result of the player is
    /// moving.
    /// </remarks>
    /// <id>
    /// 0302
    /// </id>
    public class CMSG_ESTABLISHWORLDCONNECTION : RelayPacket
    {
        public CMSG_ESTABLISHWORLDCONNECTION()
        {
        }

        public byte[] IPAddres
        {
            get
            {
                byte[] buffer = new byte[4];
                Array.Copy(this.data, 0, buffer, 0, 4);
                return buffer;
            }
        }

        public int Port
        {
            get
            {
                return BitConverter.ToInt32(this.data, 6);
            }
        }

        #region Conversions

        public static explicit operator CMSG_ESTABLISHWORLDCONNECTION(byte[] p)
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

            CMSG_ESTABLISHWORLDCONNECTION pkt = new CMSG_ESTABLISHWORLDCONNECTION();
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