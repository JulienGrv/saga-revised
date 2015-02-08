using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Authentication.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is generated when the when the client had invoked an
    /// CMSG_SERVER_REQUESTLIST Packet.
    ////
    /// Index:       a zero-based index
    /// Name:        a unicode servername
    ///
    /// Ping:        the ping value so the servers can tell if the server are
    ///              in good condition
    /// Playercount: a number representing how many characters you have on the
    ///              server.
    ///
    /// Last updated on Friday 26, okt 2007.
    /// </remarks>
    public class SMSG_SERVERLIST : RelayPacket
    {
        public SMSG_SERVERLIST()
        {
            this.Id = 0x0102;
            this.Cmd = 0x0401;
            this.data = new byte[1];
        }

        public void Add(ServerInfo e)
        {
            int offset = this.data.Length;
            Array.Resize(ref this.data, this.data.Length + 39);

            this.data[0]++;
            this.data[offset + 0] = e.Index;
            UnicodeEncoding.Unicode.GetBytes(e.Name, 0, Math.Min(16, e.Name.Length), this.data, offset + 1);
            this.data[offset + 35] = e.ping;
            this.data[offset + 36] = e.playercount;
        }
    }
}