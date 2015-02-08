using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Map
{
    internal class CMSG_CHAR_REQUESTLIST : Packet
    {
        /*
        // This packet is invoked when an client connects to the char/map server
        // and it will request an  fresh update of the character data.
        //
        // Last updated on Friday 26, okt 2007.
        */

        public CMSG_CHAR_REQUESTLIST()
        {
            this.data = new byte[4];
        }
    }
}