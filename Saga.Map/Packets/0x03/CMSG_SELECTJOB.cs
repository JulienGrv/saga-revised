using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when the click on a job in the available
    /// job-list when changing. It then calculates the costs to change to the
    /// given job and returns the costs with packet.
    /// </remarks>
    /// <id>
    /// 0311
    /// </id>
    internal class CMSG_SELECTJOB : RelayPacket
    {
        public CMSG_SELECTJOB()
        {
            this.data = new byte[0];
        }

        public byte Job
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SELECTJOB(byte[] p)
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

            CMSG_SELECTJOB pkt = new CMSG_SELECTJOB();
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