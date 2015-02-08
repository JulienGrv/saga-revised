using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Saga.Events
{
    public static partial class EventTable
    {
        public static void Participate(uint CID, byte EventId)
        {
            Character character;
            if (LifeCycle.TryGetById(CID, out character))
            {
                Predicate<byte> FindParticipatingEvent = delegate(byte evid)
                {
                    return EventId == evid;
                };

                if (character.ParticipatedEvents.Exists(FindParticipatingEvent) == false)
                {
                    SMSG_EVENTSUCCESS spkt = new SMSG_EVENTSUCCESS();
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                    character.ParticipatedEvents.Add(EventId);
                }
            }
        }
    }
}