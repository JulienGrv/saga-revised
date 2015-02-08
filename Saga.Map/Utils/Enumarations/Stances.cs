namespace Saga.Enumarations
{
    /// <summary>
    /// List of stances
    /// </summary>
    public enum StancePosition : byte
    {
        /// <summary>
        /// No stance (default is stand 3)
        /// </summary>
        None = 0,

        /// <summary>
        /// Renders the actor in a lying position
        /// </summary>
        Lie = 1,

        /// <summary>
        /// Renders the actor in sitting position
        /// </summary>
        Sit = 2,

        /// <summary>
        /// Renders the actor in a standing position
        /// </summary>
        Stand = 3,

        /// <summary>
        /// Renders the actor in a walking position
        /// </summary>
        Walk = 4,

        /// <summary>
        /// Renders the actor in a running position
        /// </summary>
        Run = 5,

        /// <summary>
        /// Renders the actor in a jumpin position
        /// </summary>
        Jump = 6,

        /// <summary>
        /// Renders the actor in a dead position
        /// </summary>
        Dead = 7,

        /// <summary>
        /// Renders the actor in a reborn position
        /// </summary>
        Reborn = 8,

        /// <summary>
        /// Renders the actor in a sitting position with hearths flowing through the air.
        /// </summary>
        /// <remarks>
        /// This stance is used by females
        /// </remarks>
        VALENTINE_SIT = 9,

        /// <summary>
        /// Renders the actor in a lying position with hearts flowing through the air.
        /// </summary>
        /// <remarks>
        /// This stance is used by male.
        /// </remarks>
        VALENTINE_LAY = 10,

        /// <summary>
        /// Renders the actor in a sitting position on a chair.
        /// </summary>
        SitOnChair = 11,

        /// <summary>
        /// Renders the actor dancing.
        /// </summary>
        Dances = 12
    };
}