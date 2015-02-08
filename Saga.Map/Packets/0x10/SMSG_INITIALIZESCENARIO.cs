using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user as a indication to initialize
    /// a scenario quest.
    /// </remarks>
    /// <id>
    /// 1001
    /// </id>
    internal class SMSG_INITIALIZESCENARIO : RelayPacket
    {
        public SMSG_INITIALIZESCENARIO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1001;
            this.data = new byte[10];
            this.StepStatus = 2;
        }

        public uint Scenario1
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint Scenario2
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }

        public byte StepStatus
        {
            set
            {
                this.data[8] = value;
            }
        }

        public byte Enabled
        {
            set
            {
                this.data[9] = value;
            }
        }
    }
}