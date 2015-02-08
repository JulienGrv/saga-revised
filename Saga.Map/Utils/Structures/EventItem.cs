namespace Saga.Structures
{
    /// <summary>
    /// Structure used database provided argument.
    /// </summary>
    public struct EventItem
    {
        /// <summary>
        /// Id of the reward
        /// </summary>
        public uint EventId;

        /// <summary>
        /// Itemid
        /// </summary>
        public uint ItemId;

        /// <summary>
        /// Number of items of item with specified itemid.
        /// </summary>
        public byte ItemCount;
    }
}