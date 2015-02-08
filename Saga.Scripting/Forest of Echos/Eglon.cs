using Saga.PrimaryTypes;
using Saga.Templates;

namespace Saga.Scripting.ForestOfEcho
{
    internal class Yordi : BaseNPC
    {
        /// <summary>
        /// Overrides the on OnGossip event so Yordi is not able
        /// to interact with a character. Just a statue.
        /// </summary>
        public override void OnGossip(Character target)
        {
            //Do nothing
        }
    }
}