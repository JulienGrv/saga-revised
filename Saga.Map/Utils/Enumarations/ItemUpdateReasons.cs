namespace Saga.Enumarations
{
    /// <summary>
    /// Structure of item update reasons
    /// </summary>
    /// <remarks>
    /// This defines wheter the client get
    /// to see player purchased x items.
    /// </remarks>
    public enum ItemUpdateReason
    {
        /// <summary>
        /// Using this doesn't show a reason
        /// </summary>
        NoReason = 0,

        /// <summary>
        /// Shows a messaga that item x has been discarded
        /// </summary>
        Discarded = 1,

        /// <summary>
        /// Shows a message that item x has been purchased
        /// </summary>
        Purchased = 2,

        /// <summary>
        /// Shows a message that item x has been sold
        /// </summary>
        Sold = 3,

        /// <summary>
        /// Shows a message that item x has been received from the npc
        /// </summary>
        ReceiveFromNpc = 4,

        /// <summary>
        /// Shows a message that item x has been given to the npc
        /// </summary>
        GiveToNpc = 5,

        /// <summary>
        /// Shows a message that you obtained a item (from looting)
        /// </summary>
        Obtained = 6,

        /// <summary>
        /// Shows a message that item x has been destroyed (for future refining of equipment)
        /// </summary>
        Destroyed = 7,

        /// <summary>
        /// Shows a message that you've registered a item on the auction
        /// </summary>
        AuctionRegister = 8,

        /// <summary>
        /// Shows a message that you've received a item from the trade
        /// </summary>
        ReceiveFromTrade = 9,

        /// <summary>
        /// Shows a message that you've given a item to a trader
        /// </summary>
        SendToTrader = 10,

        /// <summary>
        /// Shows a message that you've received a item as a quest reward
        /// </summary>
        ReceivedAsQuestReward = 11,

        /// <summary>
        /// Shows a message that you've destroyed a item because the quest canceled.
        /// </summary>
        DestroyedQuestCanceled = 12,

        /// <summary>
        /// Shows a message that you've received a item as attachment from a mail
        /// </summary>
        AttachmentReceived = 13,

        /// <summary>
        /// Shows a message that you've sent a item as attachment on a mail
        /// </summary>
        AttachmentSent = 14,

        /// <summary>
        /// Shows a message that you've received a item from storage
        /// </summary>
        StorageReceived = 15,

        /// <summary>
        /// Shows a message that you've send a item to the storage
        /// </summary>
        StorageSent = 16,

        /// <summary>
        /// Shows a message that you've receiver a item from the event
        /// </summary>
        EventReceived = 17
    }
}