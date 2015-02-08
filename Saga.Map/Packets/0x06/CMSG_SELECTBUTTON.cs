using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to indicate he or she want
    /// to use the selected button.
    /// </remarks>
    /// <id>
    /// 0615
    /// </id>
    internal class CMSG_SELECTBUTTON : RelayPacket
    {
        public CMSG_SELECTBUTTON()
        {
            this.data = new byte[1];
        }

        public byte Button
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SELECTBUTTON(byte[] p)
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

            CMSG_SELECTBUTTON pkt = new CMSG_SELECTBUTTON();
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