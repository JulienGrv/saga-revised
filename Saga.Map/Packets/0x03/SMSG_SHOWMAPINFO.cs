using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent after the maploading. It's internal data
    /// indicates which regions of the given map are visible by the player.
    /// As we all know as you progress futher in the game some part of the
    /// maps need certain maps to have been learned.
    /// </remarks>
    /// <id>
    /// 0317
    /// </id>
    internal class SMSG_SHOWMAPINFO : RelayPacket
    {
        public SMSG_SHOWMAPINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0317;
            this.data = new byte[256];
        }

        public byte[] ZoneInfo
        {
            set
            {
                Array.Copy(value, 0, this.data, 0, 256);
            }
        }
    }
}