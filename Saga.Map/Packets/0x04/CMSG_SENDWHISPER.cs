using Saga.Network;
using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Thia packet is sent to player as a result of a personal message whisper.
    /// </remarks>
    /// <id>
    /// 0402
    /// </id>
    internal class CMSG_SENDWHISPER : RelayPacket
    {
        public CMSG_SENDWHISPER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0401;
            this.data = new byte[40];
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 0, 34).TrimEnd('\0');
            }
        }

        public string Message
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 35, this.data[34]);
            }
        }

        #region Conversions

        public static explicit operator CMSG_SENDWHISPER(byte[] p)
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

            CMSG_SENDWHISPER pkt = new CMSG_SENDWHISPER();
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