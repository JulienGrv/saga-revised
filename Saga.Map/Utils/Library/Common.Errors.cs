using Saga.Packets;
using Saga.PrimaryTypes;

namespace Common
{
    public static class Errors
    {
        /// <summary>
        /// General error message
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        public static void GeneralErrorMessage(Character target, uint message)
        {
            SMSG_ERROR spkt = new SMSG_ERROR();
            spkt.SessionId = target.id;
            spkt.ErrorCode = message;
            target.client.Send((byte[])spkt);
        }

        public static void CatheleyaHealError(Character target, byte error)
        {
            SMSG_HEALRESULT spkt = new SMSG_HEALRESULT();
            spkt.SessionId = target.id;
            spkt.Result = error;
            target.client.Send((byte[])spkt);
        }
    }
}