using System;
using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Map
{

    internal class CMSG_CHAR_CREATEREPLY : RelayPacket
    {

        public CMSG_CHAR_CREATEREPLY()
        {
            this.data = new byte[4];
            this.Id = 0x0105;
        }

        public uint CharacterId
        {
            get
            {
                return BitConverter.ToUInt32(data, 0);
            }
        }

        
        #region Conversions

        public static explicit operator CMSG_CHAR_CREATEREPLY(byte[] p)
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

            CMSG_CHAR_CREATEREPLY pkt = new CMSG_CHAR_CREATEREPLY();
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
