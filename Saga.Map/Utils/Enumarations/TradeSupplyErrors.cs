namespace Saga.Enumarations
{
    /// <summary>
    /// List of trading supply errors.
    /// </summary>
    /// <remarks>
    /// These errors are beeing used by the trading npc for both coupon
    /// and normal trading functions.
    /// </remarks>
    public enum TradeSupplyErrors
    {
        /// <summary>
        /// Does not show any error message
        /// </summary>
        Success,

        /// <summary>
        /// Shows a message that your level is too low to make use of the trade
        /// </summary>
        LevelToLow,

        /// <summary>
        /// Shows a message that you don't have sufficient inventory space for the trade
        /// </summary>
        NotEnoughInventorySpace,

        /// <summary>
        /// Shows a message that you don't have enough money for the trade
        /// </summary>
        NotEnoughMoney,

        /// <summary>
        /// Shows a message that you don't have the required items for the trade
        /// </summary>
        RequiredItemsNotFound,
    }
}