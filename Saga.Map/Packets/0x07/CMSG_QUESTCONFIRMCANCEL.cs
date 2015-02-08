using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class CMSG_QUESTCONFIRMCANCEL : RelayPacket
    {
        public CMSG_QUESTCONFIRMCANCEL()
        {
            this.data = new byte[4];
        }

        public uint QuestID
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        #region Conversions

        public static explicit operator CMSG_QUESTCONFIRMCANCEL(byte[] p)
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

            CMSG_QUESTCONFIRMCANCEL pkt = new CMSG_QUESTCONFIRMCANCEL();
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