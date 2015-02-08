using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTINFO : RelayPacket
    {
        public SMSG_QUESTINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0701;
            this.data = new byte[87];
        }

        public byte count
        {
            set { this.data[0] = value; }
        }

        private int questindex = 0;

        public void AddQuest(uint QuestId, byte QuestSteps)
        {
            Array.Copy(BitConverter.GetBytes(QuestId), 0, this.data, 1 + (4 * questindex), 4);
            this.data[81 + questindex] = QuestSteps;
            Array.Resize<byte>(ref this.data, this.data.Length + (QuestSteps * 14));
            this.data[0] = (byte)++questindex;
        }

        private int offset = 87;

        public void AddQuestStep(uint StepID, byte Status, uint NextStep, bool isnew)
        {
            Array.Copy(BitConverter.GetBytes(StepID), 0, this.data, offset + 0, 4);
            this.data[offset + 4] = Status;
            Array.Copy(BitConverter.GetBytes(NextStep), 0, this.data, offset + 5, 4);
            this.data[offset + 9] = (byte)((isnew == true) ? 2 : 1);
            offset += 10;
        }
    }
}