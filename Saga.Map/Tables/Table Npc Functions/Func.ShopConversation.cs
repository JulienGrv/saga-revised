using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Structures.Collections;
using Saga.Templates;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a quest conversation. Include this in a npc to show a
    /// shop dialog.
    /// </summary>
    public class ShopConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Contains a list of items used for buying/selling.
        /// </summary>
        private BaseShopCollection _shoplist;

        /// <summary>
        /// Dialog to say after saved location
        /// </summary>
        private uint _Shop = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.Shop, new FunctionCallback(OnShop));
            OnCacheShop(npc);
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
                case "SHOP": _Shop = value; break;
            }
        }

        /// <summary>
        /// Occurs when pressing the shop buttong
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnShop(BaseNPC npc, Character target)
        {
            //Open menu
            Common.Actions.OpenMenu(
                target, npc,
                _Shop,
                DialogType.Shop,
                npc.GetDialogButtons(target)
            );

            //Open shoplist
            _shoplist.Open(target, npc);
        }

        /// <summary>
        /// Caches the shop
        /// </summary>
        /// <param name="npc">Npc who requires his shop to be cached</param>
        protected virtual void OnCacheShop(BaseNPC npc)
        {
            string filename = Server.SecurePath("~/shops/{0}.xml", npc.ModelId);
            _shoplist = ShopCollection.FromFile(filename);
        }

        #endregion Protected Methods
    }
}