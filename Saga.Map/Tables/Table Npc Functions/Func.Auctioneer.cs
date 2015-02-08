using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a auctioneer conversation. Include this in a npc to show a
    /// auctioneer dialog and submenu.
    /// </summary>
    public class AuctionConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Dialog to say when opening the auctioneer submenu
        /// </summary>
        public uint _Auction = 4462;

        /// <summary>
        /// Dialog to say when opening the auction itemlist
        /// </summary>
        public uint _AuctionOpen = 4462;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all dialogs for the npc.
        /// </summary>
        /// <param name="npc">Npc to hold the dialog</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.Market, new FunctionCallback(OnAuction));
            RegisterDialog(npc, DialogType.Market, 21, new FunctionCallback(OnAuctionOpen));
        }

        /// <summary>
        /// Shows all warp function of the current npc.
        /// </summary>
        /// <param name="npc">Npc to hold the dialog</param>
        protected internal override void OnCacheDialogInfo(BaseNPC npc, string name, uint value)
        {
            switch (name.ToUpperInvariant())
            {
                case "AUCTION": _Auction = value; break;
                case "AUCTIONOPEN": _AuctionOpen = value; break;
            }
        }

        /// <summary>
        /// Occurs when the auction button is pressed.
        /// </summary>
        /// <param name="npc">Npc who calls the function</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnAuction(BaseNPC npc, Character target)
        {
            Common.Actions.OpenSubmenu(target, npc,
                _Auction,               //Dialog script to show
                DialogType.Market,      //Button function
                21                      //Open market
            );
        }

        /// <summary>
        /// Occurs when subemnu item 'Open market' is pressed.
        /// </summary>
        /// <param name="npc">Npc who calls the function</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnAuctionOpen(BaseNPC npc, Character target)
        {
            CommonFunctions.ShowAuction(target, npc);

            Common.Actions.OpenMenu(
                target, npc,
                _AuctionOpen,
                DialogType.Market,
                new byte[] { }
            );
        }

        #endregion Protected Methods
    }
}