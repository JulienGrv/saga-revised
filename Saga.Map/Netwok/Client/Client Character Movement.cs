using System;
using System.Collections.Generic;
using System.Text;
using Saga.Packets;
using System.Threading;
using Saga.Shared.Definitions;

namespace Saga.Map.Client
{
    partial class Client
    {

        /*
        Thread MOVEMENT_THREAD;
        Stack<MapObject> APPEREANCE_QUEUE = new Stack<MapObject>();
        Queue<MapObject> UPDATE_QUEUE = new List<MapObject>();
        Queue<MapObject> DISAPPEARNCE_QUEUE = new Queue<MapObject>();


        public void INITIALIZE_MOVEMENT()
        {
            if (MOVEMENT_THREAD != null && MOVEMENT_THREAD.IsAlive == true) return;
            MOVEMENT_THREAD = new Thread(PROCESS_MOVEMENT);
        }

        public void PROCESS_MOVEMENT()
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            while (APPEREANCE_QUEUE.Count > 0)
            {
            }
            while (UPDATE_QUEUE.Count > 0)
            {
            }
            while (DISAPPEARNCE_QUEUE > 0)
            {
                MapObject current = DISAPPEARNCE_QUEUE.Dequeue();
                SMSG_ACTORDELETE spkt = new SMSG_ACTORDELETE();
                spkt.ActorID = current.id;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt2);
            }
        }
        */
    }
}
