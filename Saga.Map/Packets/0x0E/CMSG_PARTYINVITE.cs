using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the user to invite a player to a party.
    /// If the inviter is not yet in a party, a new party will be formed.
    /// </remarks>
    /// <id>
    /// 0E02
    /// </id>
    internal class CMSG_PARTYINVITE : RelayPacket
    {
        public CMSG_PARTYINVITE()
        {
            this.data = new byte[34];
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 0, 34).TrimEnd('\0');
            }
        }

        #region Conversions

        public static explicit operator CMSG_PARTYINVITE(byte[] p)
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

            CMSG_PARTYINVITE pkt = new CMSG_PARTYINVITE();
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