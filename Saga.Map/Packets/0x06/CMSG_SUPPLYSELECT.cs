using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when the user has selected
    /// item on the supply menu. This triggers
    /// </remarks>
    /// <id>
    /// 060B
    /// </id>
    internal class CMSG_SUPPLYSELECT : RelayPacket
    {
        public CMSG_SUPPLYSELECT()
        {
            this.data = new byte[8];
        }

        public uint ActorId
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        public uint ButtonId
        {
            get { return BitConverter.ToUInt32(this.data, 4); }
        }

        #region Conversions

        public static explicit operator CMSG_SUPPLYSELECT(byte[] p)
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

            CMSG_SUPPLYSELECT pkt = new CMSG_SUPPLYSELECT();
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