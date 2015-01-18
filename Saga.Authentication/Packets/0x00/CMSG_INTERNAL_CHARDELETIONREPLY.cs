using System;
using System.Collections.Generic;
using System.Text;
using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Login
{

    public class CMSG_INTERNAL_DELETIONREPLY : RelayPacket
    {

        public CMSG_INTERNAL_DELETIONREPLY()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0104;
            this.data = new byte[1];
        }

        public byte Result
        {
            get
            {
                return this.data[0]; ;
            }
        }

        #region Conversions

        public static explicit operator CMSG_INTERNAL_DELETIONREPLY(byte[] p)
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

            CMSG_INTERNAL_DELETIONREPLY pkt = new CMSG_INTERNAL_DELETIONREPLY();
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
