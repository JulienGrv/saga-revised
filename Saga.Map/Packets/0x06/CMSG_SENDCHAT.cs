
using System;
using System.Text;
using Saga.Network.Packets;

namespace Saga.Packets
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This packet is sent by a use when he or she wants says something.
    /// </remarks>
    /// <id>
    /// 0401
    /// </id>
    internal class CMSG_SENDCHAT : RelayPacket
    {        
        public CMSG_SENDCHAT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0401;
            this.data = new byte[2];
        }

        public Saga.Packets.SMSG_SENDCHAT.MESSAGE_TYPE MessageType
        {
            get
            {
                return (Saga.Packets.SMSG_SENDCHAT.MESSAGE_TYPE)this.data[0];
            }
        }

        public byte Result
        {
            get
            {
                return this.data[0];
            }
        }

        public string Message
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 2, this.data[1]);
            }
        }

        #region Conversions

        public static explicit operator CMSG_SENDCHAT(byte[] p)
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

            CMSG_SENDCHAT pkt = new CMSG_SENDCHAT();
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
