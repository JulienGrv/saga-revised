using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Map
{
    internal class CMSG_CHAR_SELECT : Packet
    {
        /*
        // This packet is invoked when an client selects a other character
        // from the Characterlist
        //
        // CharacterId containts the Unique represented id of the character
        // Channel contains the selected channel Id
        //
        // Last updated on Friday 26, okt 2007.
        */

        public CMSG_CHAR_SELECT()
        {
            this.data = new byte[5];
        }

        public int CharacterId
        {
            get
            {
                return BitConverter.ToInt32(this.data, 0);
            }
        }

        public byte Channel
        {
            get
            {
                return data[5];
            }
        }
    }
}