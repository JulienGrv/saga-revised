using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Templates;
using System.Collections.Generic;

namespace Saga.Map.Utils.Structures
{
    #region Tradinglist

    public class GroupedTradelist : BaseTradelist
    {
        protected internal List<Tradelist> list =
            new List<Tradelist>();

        public Tradelist AddList()
        {
            Tradelist mlist = new Tradelist();
            list.Add(mlist);
            return mlist;
        }

        public override void Open(Character character, BaseNPC basenpc, uint supplyid)
        {
            character.Tag = this;
            SMSG_EQUIPMENTTRADINGLIST spkt = new SMSG_EQUIPMENTTRADINGLIST();
            spkt.ActorID = basenpc.id;
            spkt.SupplyID = supplyid;
            spkt.SessionId = character.id;

            for (int i = 0; i < this.list.Count; i++)
            {
                spkt.AddItem(
                    //First production item
                    this.list[i].GetProduction(0),
                    //First production item
                    this.list[i].GetSupplement(0),
                    //Seccond production item
                    this.list[i].GetSupplement(1)
               );
            }

            character.client.Send((byte[])spkt);
        }
    }

    public class SingleTradelist : BaseTradelist
    {
        protected internal Tradelist list
            = new Tradelist();

        protected internal uint productionzeny;
        protected internal uint supplementzeny;

        public override void Open(Character character, BaseNPC basenpc, uint supplyid)
        {
            character.Tag = this;
            SMSG_SUPPLYLIST spkt2 = new SMSG_SUPPLYLIST();
            spkt2.ActorID = basenpc.id;
            spkt2.SupplyID = supplyid;

            for (int i = 0; i < this.list.GetProductionlist.Count; i++)
                spkt2.SetProducts(this.list.GetProductionlist[i]);
            for (int i = 0; i < this.list.GetSupplementlist.Count; i++)
                spkt2.SetMatrial(this.list.GetSupplementlist[i]);

            spkt2.SessionId = character.id;
            character.client.Send((byte[])spkt2);
        }
    }

    #endregion Tradinglist

    #region Guidepoints

    public abstract class GuidePoint
    {
        public GuidePoint(uint Dialog)
        {
            this.Dialog = Dialog;
        }

        public uint Dialog;
    }

    public class GuidePosition : GuidePoint
    {
        public GuidePosition(float x, float y, float z, uint Npc, uint Dialog)
            : base(Dialog)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.Npc = Npc;
        }

        public float x;
        public float y;
        public float z;
        public uint Npc;
    }

    public class GuideNpc : GuidePoint
    {
        public GuideNpc(uint Map, uint Dialog)
            : base(Dialog)
        {
            this.Map = Map;
        }

        public uint Map;
    }

    #endregion Guidepoints
}