namespace Saga.Enumarations
{
    /// <summary>
    /// Button indications
    /// </summary>
    /// <remarks>
    /// Used to indicate which button types are visible when
    /// speaking to a npc.
    /// </remarks>
    public enum DialogType : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Conversation
        /// </summary>
        EverydayConversation = 1,

        /// <summary>
        /// Location Guide
        /// </summary>
        LocationGuide,

        /// <summary>
        /// Official Quest
        /// </summary>
        OfficialQuest,

        /// <summary>
        /// Personal Quest
        /// </summary>
        PersonalQuest,

        /// <summary>
        /// Scenario Quest
        /// </summary>
        ScenarioQuest,

        /// <summary>
        /// Event Quest
        /// </summary>
        EventQuest,

        /// <summary>
        /// Open Shop
        /// </summary>
        Shop = 10,

        /// <summary>
        /// Kafra Service
        /// </summary>
        Kaftra,

        /// <summary>
        /// Auction
        /// </summary>
        Market,

        /// <summary>
        /// Skillmaster
        /// </summary>
        BookStore,

        /// <summary>
        /// Learn Skills
        /// </summary>
        U2,

        /// <summary>
        /// Enter Ship
        /// </summary>
        EnterShip,

        /// <summary>
        /// Leave Ship
        /// </summary>
        LeaveShip,

        /// <summary>
        /// Enter Train
        /// </summary>
        EnterTrain,

        /// <summary>
        /// Leave Train
        /// </summary>
        LeaveTrain,

        /// <summary>
        /// Blacksmith
        /// </summary>
        Smith = 35,

        /// <summary>
        /// Church
        /// </summary>
        Church = 41,

        /// <summary>
        /// Trade Items
        /// </summary>
        TradeItems = 42,

        /// <summary>
        /// Get Quest
        /// </summary>
        AcceptPersonalRequest = 43,

        /// <summary>
        /// Event
        /// </summary>
        EventA = 44,

        /// <summary>
        /// Event
        /// </summary>
        EventB = 45,

        /// <summary>
        /// Warp
        /// </summary>
        Warp = 46
    };
}