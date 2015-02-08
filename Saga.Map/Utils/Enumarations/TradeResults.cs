namespace Saga.Enumarations
{
    /// <summary>
    /// List of trading results
    /// </summary>
    /// <remarks>
    /// This list of errors is used by character trades not to be confused with
    /// actor trades such as the TradeSupplyErrors.
    /// </remarks>
    public enum TradeResult
    {
        /// <summary>
        /// Doesn't show a message
        /// </summary>
        Success = 0,

        /// <summary>
        /// Shows a message that the target actor wasn't found
        /// </summary>
        TargetNotFound = 1,

        /// <summary>
        /// Shows a message that the target has canceled the trade
        /// </summary>
        TargetCanceled = 2,

        /// <summary>
        /// Shows a message that you don't have enough items than you submitted.
        /// </summary>
        NotEnoughItems = 3,

        /// <summary>
        /// Shows a message that the selected item is not tradeable
        /// </summary>
        ItemNotTradeable = 4,

        /// <summary>
        /// Shows a message that you don't have the money you submitted.
        /// </summary>
        NotEnoughMoney = 5,

        /// <summary>
        /// Shows a message that you don't have enough inventory space
        /// </summary>
        NotEnoughIventorySpace = 6,

        /// <summary>
        /// Shows a message that your target doesn't have enough inventory space
        /// </summary>
        TargetNotEnoughInventorySpace = 7,

        /// <summary>
        /// Shows a message that your target is already in a trade
        /// </summary>
        TargetAlreadyInTrade = 8,

        /// <summary>
        /// Unknown trading error
        /// </summary>
        Error = 9,
    }
}