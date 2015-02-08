using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the user to indicate who should be
    /// the next party leader.
    /// </remarks>
    /// <id>
    /// 0E07
    /// </id>
    internal class CMSG_PARTYSETLEADER : RelayPacket
    {
        public CMSG_PARTYSETLEADER()
        {
            this.data = new byte[0];
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 0, 34).TrimEnd(' ');
            }
        }

        #region Conversions

        public static explicit operator CMSG_PARTYSETLEADER(byte[] p)
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

            CMSG_PARTYSETLEADER pkt = new CMSG_PARTYSETLEADER();
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