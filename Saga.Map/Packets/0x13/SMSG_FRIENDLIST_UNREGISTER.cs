using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Unregisters a existing friendlist entry</summary>
    /// <remarks>
    /// This packet is sent by the server to the client to indicate
    /// a to indicate a person should be removed from the friendlist.
    ///
    /// You can use the result field to discard the adding and show the reason
    /// why. Supported reasons: 0 Ok, 1 Character Not found, 2 List full, 3 Already added, 4 Unable to add yourself, 5 Does not exists unable to remove
    /// </remarks>
    /// <id>
    /// 1302
    /// </id>
    internal class SMSG_FRIENDLIST_UNREGISTER : RelayPacket
    {
        public SMSG_FRIENDLIST_UNREGISTER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1302;
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