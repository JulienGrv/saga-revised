using Saga.Packets;
using Saga.PrimaryTypes;
using System.Collections.Generic;

namespace Saga.Map.Utils.Structures
{
    public class JobChangeCollection
    {
        private List<byte> availablejobs;
        private List<uint> transferfee;

        public static JobChangeCollection Create(Character target)
        {
            JobChangeCollection collection = new JobChangeCollection();
            collection.availablejobs = new List<byte>();
            collection.transferfee = new List<uint>();
            for (byte i = 1; i < 7; i++)
            {
                if (i == target.job) continue;
                if (target.jlvl >= 5)
                {
                    collection.availablejobs.Add(i);
                    collection.transferfee.Add(collection.ComputeTransferFee(target, i));
                }
            }

            return collection;
        }

        protected uint ComputeTransferFee(Character character, byte job)
        {
            int Price = 0;
            Price += (character.CharacterJobLevel[job] * 300);
            List<uint> Skills = Singleton.Database.GetJobSpeciaficSkills(character, job);
            for (int i = 0; i < Skills.Count; i++)
            {
                int Lvl = (int)(Skills[i] % 100);
                Price += ((Lvl - 1) * 100);
            }

            return (uint)Price;
        }

        public bool IsJobAvailable(byte job)
        {
            for (int i = 0; i < availablejobs.Count; i++)
            {
                if (availablejobs[i] == job)
                {
                    return true;
                }
            }

            return false;
        }

        public uint GetJobTrasferFee(byte job)
        {
            for (int i = 0; i < availablejobs.Count; i++)
            {
                if (availablejobs[i] == job)
                {
                    return transferfee[i];
                }
            }

            return 0;
        }

        public void Show(Character target)
        {
            SMSG_JOBCHANGE spkt = new SMSG_JOBCHANGE();
            spkt.SessionId = target.id;
            for (int i = 0; i < availablejobs.Count; i++)
            {
                spkt.Add(availablejobs[i]);
            }
            target.Tag = this;
            target.client.Send((byte[])spkt);
        }
    }
}