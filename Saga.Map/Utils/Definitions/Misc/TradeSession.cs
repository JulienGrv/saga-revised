using Saga.PrimaryTypes;
using System;

namespace Saga.Map.Definitions.Misc
{
    public delegate void TradeHandler(Character target);

    internal class TradeSession
    {
        [Obsolete("Error", true)]
        public event TradeHandler OnCancel;

        [Obsolete("Error", true)]
        public event TradeHandler OnComplete;

        public event TradeHandler OnTradeListConfirm;

        public event TradeHandler OnTradeInvitationCancel;

        public event TradeHandler OnTradeInvitationAccept;

        public Character Source;
        public Character Target;
        public TradeItem[] SourceItem = new TradeItem[16];
        public TradeItem[] TargetItem = new TradeItem[16];
        public bool SourceHasAgreed = false;
        public bool TargetHasAgreed = true;

        public uint ZenySource;
        public uint ZenyTarget;

        public Character[] characters;
        public bool[] isconfirmed;

        public TradeSession(Character Source, Character Target)
        {
            this.characters = new Character[] { Source, Target };
            this.isconfirmed = new bool[] { false, false };
            this.Source = Source;
            this.Target = Target;
        }

        public void TradeCancel(Character target)
        {
            if (OnCancel != null) OnCancel.Invoke(target);
        }

        public void TradeConfirm(Character target)
        {
            if (target == Source) isconfirmed[0] = true;
            if (target == Target) isconfirmed[1] = true;

            if (isconfirmed[0] && isconfirmed[1])
                if (OnComplete != null)
                {
                    this.Source.ZENY -= ZenySource;
                    this.Target.ZENY -= ZenyTarget;
                    this.Source.ZENY -= ZenyTarget;
                    this.Target.ZENY -= ZenySource;
                    OnComplete.Invoke(target);
                }
        }

        public void TradeListConfirm(Character target)
        {
            if (OnTradeListConfirm != null) OnTradeListConfirm.Invoke(target);
        }

        public void TradeInvitationAccept(Character target)
        {
            if (OnTradeInvitationAccept != null) OnTradeInvitationAccept.Invoke(target);
        }

        public void TradeInvitationCancel(Character target)
        {
            if (OnTradeInvitationCancel != null) OnTradeInvitationCancel.Invoke(target);
        }

        public class TradeItem
        {
            public byte Slot;
            public byte Count;
        }
    }
}