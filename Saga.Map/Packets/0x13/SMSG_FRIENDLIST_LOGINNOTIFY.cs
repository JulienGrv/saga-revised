using Saga.Network.Packets;
using System.Text;

namespace Saga.Packets
{
    /// <summary>Notification of friend login</summary>
    /// <remarks>
    /// This packet is sent by the server to the client to notify that their friend has loged on.
    /// The person will only see the msg displayed if they have the option enabled client side to be notified
    /// of friend logons.
    ///
    /// The packet I had captured had 23 bytes of 0x00 after the friend name.
    /// Another packet was similiar but last byte was 0x01... perhaps friend log off packet?
    /// This implementation currently doesnt include that empty wasted space and still works.
    /// </remarks>
    /// <id>
    /// 1307
    /// </id>
    internal class SMSG_FRIENDSLIST_NOTIFYLOGIN : RelayPacket
    {
        public SMSG_FRIENDSLIST_NOTIFYLOGIN()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1307;
            this.data = new byte[60];
        }

        public string name
        {
            set
            {
                UnicodeEncoding.Unicode.GetBytes(value, 0, value.Length, this.data, 0);//Math.Min(value.Length, 16)<--is this for security purposes? if not, its an extra, unneeded call
            }
        }
    }
}