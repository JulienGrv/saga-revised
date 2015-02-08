using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is invoked by the client when he/she
    /// Tries to authenticate with the server.
    ///
    /// Username a string containing the username
    /// Password a string containing the md5-hash of the password
    /// </remarks>
    public class CMSG_REQUESTLOGIN : Packet
    {
        public CMSG_REQUESTLOGIN()
        {
            this.data = new byte[100];
        }

        public string Username
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 4, 34);
            }
        }

        public string Password
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 38, Math.Max(0, this.data.Length - 38));
            }
        }

        public static explicit operator byte[](CMSG_REQUESTLOGIN p)
        {
            byte[] tmp = new byte[p.Size];
            Array.Copy(BitConverter.GetBytes(p.Size), 0, tmp, 0, 2);
            Array.Copy(BitConverter.GetBytes(p.session), 0, tmp, 2, 4);
            Array.Copy(p.id, 0, tmp, 6, 2);
            Array.Copy(p.data, 0, tmp, 10, p.data.Length);
            return tmp;
        }

        public static explicit operator CMSG_REQUESTLOGIN(byte[] p)
        {
            CMSG_REQUESTLOGIN pkt = new CMSG_REQUESTLOGIN();
            pkt.data = new byte[p.Length - 10];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.id, 0, 2);
            Array.Copy(p, 10, pkt.data, 0, p.Length - 10);
            return pkt;
        }
    }
}