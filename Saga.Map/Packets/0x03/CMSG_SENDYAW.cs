using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a result of his direction changes.
    /// </remarks>
    /// <id>
    /// 0304
    /// </id>
    internal class CMSG_SENDYAW : RelayPacket
    {
        public CMSG_SENDYAW()
        {
            this.data = new byte[0];
        }

        public Rotator Yaw
        {
            get
            {
                return new Rotator(
                    BitConverter.ToUInt16(this.data, 0),
                    BitConverter.ToUInt16(this.data, 2));
            }
        }

        #region Conversions

        public static explicit operator CMSG_SENDYAW(byte[] p)
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

            CMSG_SENDYAW pkt = new CMSG_SENDYAW();
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