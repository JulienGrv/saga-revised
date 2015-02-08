using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;
using System.Collections.Generic;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a warper conversation. Include this in a npc to show a
    /// warper dialog and submenu.
    /// </summary>
    /// <remarks>
    /// This function will read a xml file: warpers/{npcmodelid}.xml from the
    /// data folder. The items in the xml file will be used to determine which
    /// warpspots are available for the npc.
    /// </remarks>
    public class WarperConversation : NpcFunction
    {
        #region Protected Methods

        /// <summary>
        /// Registers all dialogs for the npc.
        /// </summary>
        /// <param name="npc">Npc to hold the dialog</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.Warp, new FunctionCallback(OnWarp));
        }

        /// <summary>
        /// Shows all warp function of the current npc.
        /// </summary>
        /// <param name="npc">Npc to hold the dialog</param>
        protected virtual void OnWarp(BaseNPC npc, Character target)
        {
            CommonFunctions.ShowWarpOptions(target, npc,
                GetWarpLocations(npc)
            );
        }

        /// <summary>
        /// Return all the wap locations availabel of the current npc.
        /// </summary>
        /// <param name="npc">Npc which to find the warps for</param>
        /// <returns></returns>
        protected virtual IEnumerable<ushort> GetWarpLocations(BaseNPC npc)
        {
            return Common.Special.GetWarperTable(npc.ModelId);
        }

        #endregion Protected Methods
    }
}