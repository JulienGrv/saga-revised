using Saga.Data;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.PrimaryTypes;
using Saga.Structures;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Saga.Map
{
    public interface IInfoProvider2
    {
        IDataWeaponCollection createWeaponCollection();

        IDataCharacter createDataCharacter();

        IDataAdditionCollection createAdditionCollection();

        IDataSortableItemCollection createInventoryCollection();

        IDataSortableItemCollection createStorageCollection();

        IDataJobinformationCollection createJobCollection();

        IDataZoneInformationCollection createDataZoneCollection();

        IDataEquipmentCollection createEquipmentCollection();

        IDataSkillCollection createSkillCollection();

        IDatabaseQuestStream createDatabaseQuestStream();

        IDataSpecialSkillCollection createDatabaseSpecialSkillCollection();

        IDatabaseFriendList createDatabaseFriendList();

        IDatabaseBlacklist createDatabaseBlacklist();

        uint OwnerId { get; set; }
    }

    public interface IDataWeaponCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's or Sets a weapon at the specified index
        /// </summary>
        /// <param name="index">Zero based index where to set the weapon</param>
        /// <returns>Weapon</returns>
        Weapon this[int index] { get; set; }

        /// <summary>
        /// Get's or set's the number of unlocked weapon slots (maximum 5)
        /// </summary>
        byte UnlockedWeaponSlots { get; set; }

        /// <summary>
        /// Get's or set's the index of the primary weapon
        /// </summary>
        /// <remarks>
        /// Set this value to 255 to use hands. A value between 0 - 5 will
        /// try to get the weapon at the specified index.
        /// </remarks>
        byte PrimaryWeaponIndex { get; set; }

        /// <summary>
        /// Get's or set's the index of the seccondairy weapon
        /// </summary>
        /// <remarks>
        /// Set this value to 255 to use hands. A value between 0 - 5 will
        /// try to get the weapon at the specified index.
        /// </remarks>
        byte SeconairyWeaponIndex { get; set; }

        /// <summary>
        /// Get's or sets the active weapon index
        /// </summary>
        /// <remarks>
        /// Use 0 for using the left hand and 1 for using the right hand.
        /// </remarks>
        byte ActiveWeaponIndex { get; set; }
    }

    public interface IDataCharacter
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; set; }

        /// <summary>
        /// Get's or Set's the character name
        /// </summary>
        string CharacterName { get; set; }

        /// <summary>
        /// Get's or sets the characters face details
        /// </summary>
        byte[] CharacterFace { get; }

        /// <summary>
        /// Get's or set the character experience also known as base experience or cexp
        /// </summary>
        uint CharacterExperience { get; set; }

        /// <summary>
        /// Get's or set the job experience also known as jexp
        /// </summary>
        uint JobExperience { get; set; }

        /// <summary>
        /// Get's or set's the current job
        /// </summary>
        byte Job { get; set; }

        /// <summary>
        /// Get's or set's the current hp of the character
        /// </summary>
        ushort HP { get; set; }

        /// <summary>
        /// Get's or set's the current sp of the character
        /// </summary>
        ushort SP { get; set; }

        /// <summary>
        /// Get's or set's the current lp of the character
        /// </summary>
        byte LP { get; set; }

        /// <summary>
        /// Get's or set's the current breath of the character
        /// </summary>
        byte Oxygen { get; set; }

        /// <summary>
        /// Get's or set's the current strenght stat
        /// </summary>
        ushort Strength { get; set; }

        /// <summary>
        /// Get's or set's the current dexterity stat
        /// </summary>
        ushort Dexterity { get; set; }

        /// <summary>
        /// Get's or set's the current intellect stat
        /// </summary>
        ushort Intellect { get; set; }

        /// <summary>
        /// Get's or set's the current concentration stat
        /// </summary>
        ushort Concentration { get; set; }

        /// <summary>
        /// Get's or set's the current luck stat
        /// </summary>
        ushort Luck { get; set; }

        /// <summary>
        /// Get's or set's the current remainging stat points
        /// </summary>
        ushort Remaining { get; set; }

        /// <summary>
        /// Get's or set's the character amount of zeny
        /// </summary>
        uint Zeny { get; set; }

        WorldCoordinate Position
        {
            get;
            set;
        }

        WorldCoordinate SavePosition
        {
            get;
            set;
        }
    }

    public interface IDataAdditionCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Creates a addition
        /// </summary>
        /// <param name="addition">unique id of the addition</param>
        /// <param name="duration">time spawn for the addition</param>
        /// <returns>True if the addition was loaded</returns>
        bool Create(uint addition, uint duration);

        /// <summary>
        /// Get's all addition
        /// </summary>
        IEnumerable<AdditionState> Additions { get; }
    }

    public interface IDataSortableItemCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's or set's the sortation mode
        /// </summary>
        byte SortationMode { get; set; }

        /// <summary>
        /// Get's container for the items
        /// </summary>
        Rag2Collection Collection { get; }
    }

    public interface IDataJobinformationCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's container for all the joblevels
        /// </summary>
        byte[] Joblevels { get; }
    }

    public interface IDataZoneInformationCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's container for all the zone information
        /// </summary>
        byte[] ZoneInformation { get; }
    }

    public interface IDataEquipmentCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's container for all the zone information
        /// </summary>
        Rag2Item[] Equipment { get; }
    }

    public interface IDataSkillCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's the job id of the character
        /// </summary>
        byte Job { get; }

        /// <summary>
        /// Get's container for all the skills
        /// </summary>
        List<Skill> Skills { get; }
    }

    public interface IDataSpecialSkillCollection
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's the special skill collection (max size of 16 elements)
        /// </summary>
        Skill[] specialSkillCollection { get; }
    }

    public interface IDatabaseQuestStream
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's or set's the quest data stream
        /// </summary>
        byte[] questCollection { get; set; }
    }

    public interface IDatabaseFriendList
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's a list of friends
        /// </summary>
        List<string> friends { get; }
    }

    public interface IDatabaseBlacklist
    {
        /// <summary>
        /// Get's the character id of the owner
        /// </summary>
        uint CharacterId { get; }

        /// <summary>
        /// Get's a list of blacklisted people
        /// </summary>
        List<KeyValuePair<string, byte>> blacklist { get; }
    }

    public interface IDatabase
    {
        #region General

        /// <summary>
        /// Checks the server version.
        /// </summary>
        bool CheckServerVersion();

        /// <summary>
        /// Summary on connect
        /// </summary>
        /// <param name="info">Connection info to establish the connection</param>
        bool Connect(ConnectionInfo info);

        /// <summary>
        /// Checks the database for missing tables and fields
        /// </summary>
        bool CheckDatabaseFields();

        #endregion General

        #region Skills

        void LoadSkills(Character character, uint CharId);

        IEnumerable<uint> GetAllLearnedSkills(Character target);

        bool InsertNewSkill(uint CharId, uint SkillId, byte job);

        bool UpdateSkill(Character target, uint SkillId, uint Experience);

        bool UpgradeSkill(Character target, uint OldSkillId, uint NewSkillId, uint Experience);

        bool InsertNewSkill(uint CharId, uint SkillId, byte job, uint Experience);

        List<uint> GetJobSpeciaficSkills(Character target, byte job);

        #endregion Skills

        #region Mailbox

#pragma warning disable 0436

        IEnumerable<Mail> GetInboxMail(Character target);

        IEnumerable<Mail> GetOutboxMail(Character target);

#pragma warning restore 0436

        int GetInboxMailCount(string name);

        int GetOutboxMailCount(Character target);

        int GetInboxUncheckedCount(string name);

        bool GetItemByAuctionId(uint id, out AuctionArgument item);

        bool DeleteRegisteredAuctionItem(uint id);

        bool InsertNewMailItem(Character target, MailItem item);

        MailItem GetMailItemById(uint id);

        IEnumerable<KeyValuePair<string, uint>> GetPendingMails();

        bool ClearPendingMails(string Receiptent);

        bool MarkAsReadMailItem(uint id);

        uint GetZenyAttachment(uint id);

        bool UpdateZenyAttachment(uint Id, uint Zeny);

        bool DeleteMails(uint id);

        bool DeleteMailFromOutbox(uint id);

        bool UpdateItemAttachment(uint MailId, Rag2Item Attachment);

        Rag2Item GetItemAttachment(uint MailId);

        #endregion Mailbox

        #region Auction

        /// <summary>
        /// Search for all market items registered in the database.
        /// </summary>
        /// <param name="Argument">Search argument</param>
        /// <returns>Iterable list of MarketItemArguments</returns>
        IEnumerable<MarketItemArgument> SearchMarket(MarketSearchArgument Argument);

        /// <summary>
        /// Search for market items registered by the specified character.
        /// </summary>
        /// <param name="target">Character who the items are from</param>
        /// <returns>Iterable list of MarketItemArguments</returns>
        IEnumerable<MarketItemArgument> SearchMarketForOwner(Character target);

        /// <summary>
        /// Registers a new marketitem on the database
        /// </summary>
        /// <param name="target">Character who the database is for.</param>
        /// <param name="arg">Item argument</param>
        /// <returns></returns>
        uint RegisterNewMarketItem(Character target, MarketItemArgument arg);

        /// <summary>
        /// Unregisters the selected market id.
        /// </summary>
        /// <param name="id">Id of the marketitem</param>
        /// <returns>Number of items registered</returns>
        uint UnregisterMarketItem(uint id);

        /// <summary>
        /// Find a comment by the associated itemid.
        /// </summary>
        /// <param name="id">Id of the marketitem</param>
        /// <returns>Display comment</returns>
        string FindCommentByPlayerId(uint id);

        /// <summary>
        /// Find a comment by the associated itemid.
        /// </summary>
        /// <param name="id">Id of the marketitem</param>
        /// <returns>Display comment</returns>
        string FindCommentById(uint id);

        /// <summary>
        /// Update the comment by the specified player id.
        /// </summary>
        /// <param name="id">Id of the player</param>
        /// <param name="message">Message to display in the future</param>
        /// <returns>Number of updated records</returns>
        uint UpdateCommentByPlayerId(uint id, string message);

        /// <summary>
        /// Gets the count of registered items by the owner character
        /// </summary>
        /// <param name="target">Character who's items to count</param>
        /// <returns>Number of items found</returns>
        int GetOwnerItemCount(Character target);

        #endregion Auction

        #region Quests

        IEnumerable<uint> GetAvailableQuestsByRegion(Character target, uint modelid);

        IEnumerable<KeyValuePair<uint, uint>> GetPersonalAvailableQuestsByRegion(Character target, byte region, uint CurrentPersonalQuest);

        bool QuestComplete(uint charId, uint QuestId);

        bool IsQuestComplete(uint charId, uint QuestId);

        #endregion Quests

        #region FriendList

        /// <summary>
        /// Delete a character on the friendlist of the user.
        /// </summary>
        /// <param name="charId">CharacterId if of the character</param>
        /// <param name="friend">Name of the foe</param>
        /// <returns></returns>
        bool DeleteFriend(uint charId, string friend);

        /// <summary>
        /// Delete a character on the blacklist of the user.
        /// </summary>
        /// <param name="charId">CharacterId if of the character</param>
        /// <param name="friend">Name of the foe</param>
        /// <returns></returns>
        bool DeleteBlacklist(uint charId, string friend);

        /// <summary>
        /// Adds the specified charactername on the friendlist
        /// of the character.
        /// </summary>
        /// <param name="charId">CharacterId if of the character</param>
        /// <param name="friend">Name of the friend</param>
        /// <returns></returns>
        bool InsertAsFriend(uint charId, string friend);

        /// <summary>
        /// Adds the specified charactername on the blacklist
        /// of the character.
        /// </summary>
        /// <param name="charId">CharacterId if of the character</param>
        /// <param name="friend">Name of the foe</param>
        /// <param name="reason">Reason for the ban</param>
        /// <returns>False on database error</returns>
        bool InsertAsBlacklist(uint charId, string friend, byte reason);

        #endregion FriendList

        #region Character

        IEnumerable<CharInfo> FindCharacters(uint PlayerId);

        bool FindCharacterDetails(uint CharId, out CharDetails details);

        bool VerifyNameExists(string name);

        bool DeleteCharacterById(uint id);

        bool GetPlayerId(string name, out uint charid);

        #endregion Character

        #region Event

        /// <summary>
        /// Retrieves a list of item rewards that were given by
        /// to the specified character
        /// </summary>
        /// <param name="target">Character instance to retrieve</param>
        /// <returns>A collection of EventItemLists</returns>
        Collection<EventItem> FindEventItemList(Character target);

        /// <summary>
        /// Retrieves a even item reward based on the given unqiue id,
        /// </summary>
        /// <param name="target">Owner character of the item</param>
        /// <param name="RewardId">Unique id of the reward</param>
        /// <returns>a single eventitem reward</returns>
        EventItem FindEventItemById(Character target, uint RewardId);

        /// <summary>
        /// Deletes a event item reward based on the given unique id.
        /// </summary>
        /// <param name="RewardId"></param>
        /// <returns></returns>
        bool DeleteEventItemId(uint RewardId);

        /// <summary>
        /// Creates an event item
        /// </summary>
        /// <returns>True if item was made</returns>
        bool CreateEventItem(uint charid, uint Itemid, byte stackcount);

        #endregion Event

        #region Plugin

        IQueryProvider GetQueryProvider();

        IDataReader ExecuteDataReader(IQueryProvider query);

        IDataReader ExecuteDataReader(IQueryProvider query, CommandBehavior behavior);

        int ExecuteNonQuery(IQueryProvider query);

        #endregion Plugin

        //Third generation methods
        bool TransactionSave(IInfoProvider2 dbq);

        bool TransactionLoad(IInfoProvider2 dbq, bool continueOnError);

        bool TransactionRepair(IInfoProvider2 dbq);

        bool TransactionInsert(IInfoProvider2 dbq);

        bool GetCharacterId(string name, out uint charId);
    }
}