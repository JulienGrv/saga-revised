using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    public class CMSG_DELETECHARACTER : RelayPacket
    {
        /*
        // This packet is invoked when an client deletes the specified
        // character on.
        //
        // Index contains an zero-based offset that specifies the character
        // Name speciafies the name of the character.
        //
        // Last updated on Friday 26, okt 2007.
        */

        public CMSG_DELETECHARACTER()
        {
            this.data = new byte[35];
            this.Cmd = 0x0401;
            this.Id = 0x108;
        }

        public byte Index
        {
            get
            {
                return data[0];
            }
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 1, 34).TrimEnd('\0');
            }
        }

        #region Conversions

        public static explicit operator CMSG_DELETECHARACTER(byte[] p)
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

            CMSG_DELETECHARACTER pkt = new CMSG_DELETECHARACTER();
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