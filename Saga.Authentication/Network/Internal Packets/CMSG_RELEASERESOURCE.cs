using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    public class CMSG_RELEASERESOURCES : RelayPacket
    {
        public CMSG_RELEASERESOURCES()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0003;
        }

        public uint Session
        {
            get
            {
                return BitConverter.ToUInt32(this.data, 0);
            }
        }

        #region Conversions

        public static explicit operator CMSG_RELEASERESOURCES(byte[] p)
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

            CMSG_RELEASERESOURCES pkt = new CMSG_RELEASERESOURCES();
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