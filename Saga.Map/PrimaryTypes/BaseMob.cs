using Saga.Enumarations;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;

namespace Saga.Map
{
    public abstract class BaseMob : Actor
    {
        #region Appearance

        public virtual int ComputeIcon(Character character)
        {
            return (stance == 7) ? 4 : 0;
        }

        public virtual int ComputeIsAggresive(Character character)
        {
            return 0;
        }

        #endregion Appearance

        #region Position

        public void WideMovement(params WaypointStructure[] waypoints)
        {
            SMSG_WIDEMOVEMENTSTART spkt = new SMSG_WIDEMOVEMENTSTART();
            spkt.SourceActorID = this.id;
            spkt.Speed = (ushort)this.Status.WalkingSpeed;
            for (int i = 0; i < waypoints.Length; i++)
                spkt.AddWaypoint(waypoints[i].point, waypoints[i].rotation);

            Regiontree tree = this.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(SearchFlags.Characters))
            {
                if (character.client.isloaded == false) continue;
                spkt.SessionId = character.id;
                character.client.Send((byte[])spkt);
            }
        }

        public void WideMovement(Character character, params WaypointStructure[] waypoints)
        {
            SMSG_WIDEMOVEMENTSTART spkt = new SMSG_WIDEMOVEMENTSTART();
            spkt.SourceActorID = this.id;
            spkt.Speed = (ushort)this.Status.WalkingSpeed;
            for (int i = 0; i < waypoints.Length; i++)
                spkt.AddWaypoint(waypoints[i].point, waypoints[i].rotation);
            spkt.SessionId = character.id;
            character.client.Send((byte[])spkt);
        }

        #endregion Position

        #region Base Members

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override void ShowObject(Character character)
        {
            SMSG_NPCINFO spkt2 = new SMSG_NPCINFO(1);
            lock (this)
            {
                spkt2.ActorID = this.id;
                spkt2.NPCID = this.ModelId;
                spkt2.SessionId = character.id;
                spkt2.SP = this.SP;
                spkt2.MaxSP = this.SPMAX;
                spkt2.HP = this.HP;
                spkt2.MaxHP = this.HPMAX;
                spkt2.Yaw = this.Yaw;
                spkt2.X = this.Position.x;
                spkt2.Y = this.Position.y;
                spkt2.Z = this.Position.z;
                spkt2.IsAggresive = (byte)this.ComputeIsAggresive(character);
                spkt2.Icon = (byte)this.ComputeIcon(character);
            }
            character.client.Send((byte[])spkt2);
        }

        #endregion Base Members
    }
}