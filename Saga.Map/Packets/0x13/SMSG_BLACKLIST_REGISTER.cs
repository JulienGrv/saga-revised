using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Registers new blacklist entry</summary>
    /// <remarks>
    /// This packet is sent by the server to the client as a response on CMSG_BLACKLIST_REGISTER to indicate
    /// a new character should be added to the blacklist. Note that this will add 1 character
    /// incremental to the list.
    ///
    /// You can use the result field to discard the adding and show the reason
    /// why. Supported reasons: 0 Ok, 1 Character Not found, 2 List full, 3 Already added, 4 Unable to add yourself, 5 Does not exists unable to remove
    /// </remarks>
    /// <id>
    /// 1304
    /// </id>
    internal class SMSG_BLACKLIST_REGISTER : RelayPacket
    {
        public SMSG_BLACKLIST_REGISTER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1304;
            this.data = new byte[37];
        }

        public string Name
        {
            set
            {
                UnicodeEncoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 0);
            }
        }

        public byte Result
        {
            set
            {
                this.data[36] = value;
            }
        }

        public byte Reason
        {
            set
            {
                this.data[35] = value;
            }
        }
    }
}