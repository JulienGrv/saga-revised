using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class CMSG_SKILLCAST : RelayPacket
    {
        public CMSG_SKILLCAST()
        {
            this.data = new byte[9];
        }

        public byte SkillType
        {
            get { return this.data[0]; }
        }

        public uint SkillID
        {
            get { return BitConverter.ToUInt32(this.data, 1); }
        }

        public uint TargetActor
        {
            get { return BitConverter.ToUInt32(this.data, 5); }
        }

        #region Conversions

        public static explicit operator CMSG_SKILLCAST(byte[] p)
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

            CMSG_SKILLCAST pkt = new CMSG_SKILLCAST();
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