using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>Take screenshot</summary>
    /// <remarks>
    /// This packet is sent by the server as a response
    /// on packet CMSG_TAKESCREENSHOT. By responding with
    /// this packet the server knows the screenshots are taken.
    /// </remarks>
    /// <id>
    /// 1101
    /// </id>
    internal class SMSG_SCREENSHOTALLOWED : RelayPacket
    {
        public SMSG_SCREENSHOTALLOWED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1202;
            this.data = new byte[0];
        }
    }

    internal class SMSG_SHOWCHANNELS : RelayPacket
    {
        public SMSG_SHOWCHANNELS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1201;
            this.data = new byte[12];
        }

        public void Add(byte channel, byte state)
        {
            //int index = this.data.Length;
            //Array.Resize<byte>(ref this.data, index + 1);
            int index = 2 + (channel * 2);
            this.data[index] = channel;
            this.data[index + 1] = state;
            this.data[1]++;
        }
    }
}