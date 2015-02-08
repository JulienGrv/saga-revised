using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Unregisters a blacklist entry</summary>
    /// <remarks>
    /// This packet is sent by the server to the client to indicate
    /// a to indicate a person should be removed from the blacklist.
    ///
    /// You can use the result field to discard the adding and show the reason
    /// why. Supported reasons: 0 Ok, 1 Character Not found, 2 List full, 3 Already added, 4 Unable to add yourself, 5 Does not exists unable to remove
    /// </remarks>
    /// <id>
    /// 1305
    /// </id>
    internal class SMSG_BLACKLIST_UNREGISTER : RelayPacket
    {
        public SMSG_BLACKLIST_UNREGISTER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1305;
            this.data = new byte[37];
        }

        public string Name
        {
            set
            {
                UnicodeEncoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 0);
            }
        }

        public byte Reason
        {
            set
            {
                this.data[34] = value;
            }
        }
    }
}