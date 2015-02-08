using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Login
{
    public class CMSG_FINDCHARACTERS : RelayPacket
    {
        public CMSG_FINDCHARACTERS()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0002;
            this.data = new byte[4];
        }

        public byte Count
        {
            get
            {
                return this.data[0];
            }
        }

        private int offset = 1;

        public void GetCharInfo(out CharInfo info)
        {
            info = new CharInfo();
            info.charId = BitConverter.ToUInt32(this.data, offset);
            info.name = UnicodeEncoding.Unicode.GetString(this.data, offset + 4, 32).TrimEnd('\0');
            info.cexp = BitConverter.ToUInt32(this.data, offset + 39);
            info.job = this.data[offset + 43];
            offset += 46;
        }

        #region Conversions

        public static explicit operator CMSG_FINDCHARACTERS(byte[] p)
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

            CMSG_FINDCHARACTERS pkt = new CMSG_FINDCHARACTERS();
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