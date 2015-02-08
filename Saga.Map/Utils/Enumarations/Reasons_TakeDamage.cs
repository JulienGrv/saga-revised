namespace Saga.Enumarations
{
    /// <summary>
    /// Reasons for taking damage.
    /// </summary>
    /// <remarks>
    /// This list is mainly used by either fall or oxygen related damage.
    /// </remarks>
    public enum TakeDamageReason
    {
        /// <summary>
        /// Shows a message that you've taken damage because you fall
        /// </summary>
        Falling = 1,

        /// <summary>
        /// Shows a message that you've taken damage because you lack oxygen
        /// </summary>
        Oxygen,

        /// <summary>
        /// Shows a message that you've taken damage due a fall but barely survived
        /// </summary>
        Survive,

        /// <summary>
        /// Shows a message that you've fallen and died
        /// </summary>
        FallenDead,

        /// <summary>
        /// Shows a message that you lack oxygen and died
        /// </summary>
        Suffocated
    }
}