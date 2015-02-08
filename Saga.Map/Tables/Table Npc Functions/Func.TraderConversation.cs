using Saga.Enumarations;
using Saga.Map.Utils.Structures;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a trader conversation. Include this in a npc to show a
    /// trader dialog and submenus.
    /// </summary>
    public class TraderConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Container for trade items.
        /// </summary>
        /// <remarks>
        /// Prior to opening the trade window this item is set to this
        /// list. Our core will handle all the trades.
        /// </remarks>
        protected TradelistContainer _container;

        /// <summary>
        /// Dialog to show when opening the trader conversation
        /// </summary>
        protected uint _TradeDialog = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.TradeItems, new FunctionCallback(OnTradeDialog));
            OnCacheTrader(npc);
        }

        /// <summary>
        /// Refreshes the trader
        /// </summary>
        /// <param name="npc">Target npc</param>
        protected internal override void OnRefresh(BaseNPC npc)
        {
            OnCacheTrader(npc);
        }

        /// <summary>
        /// Loads all dialog dialog information.
        /// </summary>
        /// <remarks>
        /// The information is loaded from the data folder in dialogtemplates/{npcmodelid}.xml
        /// </remarks>
        /// <param name="npc">Npc who requires caching</param>
        /// <param name="name">Name of the found string</param>
        /// <param name="value">Value of the found string</param>
        protected internal override void OnCacheDialogInfo(BaseNPC npc, string name, uint value)
        {
            switch (name.ToUpperInvariant())
            {
                case "TRADEDIALOG": _TradeDialog = value; break;
            }
        }

        /// <summary>
        /// Occurs after showing the trader dialog.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="target"></param>
        protected virtual void OnTradeDialog(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(
                target, npc,
                _TradeDialog,
                DialogType.TradeItems,
                npc.GetDialogButtons(target)
            );

            _container.Open(target, npc);
        }

        /// <summary>
        /// Caches the trader object form a file.
        /// </summary>
        /// <param name="npc"></param>
        protected virtual void OnCacheTrader(BaseNPC npc)
        {
            string filename = Server.SecurePath("~/traders/{0}.xml", npc.ModelId);
            _container = TradelistContainer.FromFile(filename);
        }

        #endregion Protected Methods
    }
}