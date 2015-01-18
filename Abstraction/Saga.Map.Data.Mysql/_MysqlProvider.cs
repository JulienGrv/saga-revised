using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Saga.Data;
using Saga.Map.Utils.Definitions.Misc;
using Saga.PrimaryTypes;
using Saga.Structures;

namespace Saga.Map.Data.Mysql
{
    public partial class MysqlProvider : IDatabase
    {

        #region Junk

        bool IDatabase.Connect(ConnectionInfo info)
        {
            return Connect(info);
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.CheckServerVersion()
        {
            MySqlConnection connection = ConnectionPool.Request();        
            try
            {
                return CheckServerVersion(connection);
            }
            catch (Exception)
            {                
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.ClearPendingMails(string Receiptent)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return ClearPendingMails(connection, Receiptent);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }            
        }        

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteBlacklist(uint charId, string friend)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteBlacklist(connection, charId, friend);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            } 
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteCharacterById(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteCharacterById(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }             
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteFriend(uint charId, string friend)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteFriend(connection, charId, friend);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }  
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteMailFromOutbox(uint id)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteMailFromOutbox(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteMails(uint id)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteMails(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.DeleteRegisteredAuctionItem(uint id)
        {

            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return DeleteRegisteredAuctionItem(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }        

        /// <Safe>Yes</Safe>
        bool IDatabase.FindCharacterDetails(uint CharId, out CharDetails details)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return FindCharacterDetails(connection, CharId, out details);
            }
            catch (Exception)
            {
                details = new CharDetails();
                Trace.TraceWarning("Unhandeled database error");
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        IEnumerable<CharInfo> IDatabase.FindCharacters(uint PlayerId)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return FindCharacters(connection, PlayerId);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        string IDatabase.FindCommentById(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return FindCommentById(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        string IDatabase.FindCommentByPlayerId(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return FindCommentByPlayerId(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }            
        }

        /// <Safe>Yes</Safe>
        IEnumerable<uint> IDatabase.GetAllLearnedSkills(Character target)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetAllLearnedSkills(connection, target);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        IEnumerable<uint> IDatabase.GetAvailableQuestsByRegion(Character target, uint modelid)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetAvailableQuestsByRegion(connection, target, modelid);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        IEnumerable<Saga.Map.Definitions.Misc.Mail> IDatabase.GetInboxMail(Character target)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetInboxMail(connection, target);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        int IDatabase.GetInboxMailCount(string name)
        {            
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetInboxMailCount(connection, name);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return 0;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        int IDatabase.GetInboxUncheckedCount(string name)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetInboxUncheckedCount(connection, name);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return 0;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.GetItemByAuctionId(uint id, out AuctionArgument item)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetItemByAuctionId(connection, id, out item);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                item = null;
                return false;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        List<uint> IDatabase.GetJobSpeciaficSkills(Character target, byte job)
        {           
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetJobSpeciaficSkills(connection, target, job);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return new List<uint>();
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        Saga.Map.Utils.Definitions.Misc.MailItem IDatabase.GetMailItemById(uint id)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return GetMailItemById(connection, id);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return null;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }

        /// <Safe>Yes</Safe>
        bool IDatabase.IsQuestComplete(uint charId, uint QuestId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return IsQuestComplete(connection, charId, QuestId);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Unhandeled database error");
                return true;
            }
            finally
            {
                //Release resources
                ConnectionPool.Release(connection);
            }
        }



        IEnumerable<Saga.Map.Definitions.Misc.Mail> IDatabase.GetOutboxMail(Character target)
        {
            return GetOutboxMail(target);
        }

        int IDatabase.GetOutboxMailCount(Character target)
        {
            return GetOutboxMailCount(target);
        }

        int IDatabase.GetOwnerItemCount(Character target)
        {
            return GetOwnerItemCount(target);
        }

        IEnumerable<KeyValuePair<string, uint>> IDatabase.GetPendingMails()
        {
            return GetPendingMails();
        }

        IEnumerable<KeyValuePair<uint, uint>> IDatabase.GetPersonalAvailableQuestsByRegion(Character target, byte region, uint CurrentPersonalQuest)
        {
            return GetPersonalAvailableQuestsByRegion(target, region, CurrentPersonalQuest);
        }


        Saga.Data.IQueryProvider IDatabase.GetQueryProvider()
        {
            return GetQueryProvider();
        }

        uint IDatabase.GetZenyAttachment(uint id)
        {
            return GetZenyAttachment(id);
        }

        bool IDatabase.InsertAsBlacklist(uint charId, string friend, byte reason)
        {
            return InsertAsBlacklist(charId, friend, reason);
        }

        bool IDatabase.InsertAsFriend(uint charId, string friend)
        {
            return InsertAsFriend(charId, friend);
        }


        bool IDatabase.InsertNewMailItem(Character target, MailItem item)
        {
            return InsertNewMailItem(target, item);
        }

        bool IDatabase.InsertNewSkill(uint CharId, uint SkillId, byte job, uint Experience)
        {
            return InsertNewSkill(CharId, SkillId, job, Experience);
        }

        bool IDatabase.InsertNewSkill(uint CharId, uint SkillId, byte job)
        {
            return InsertNewSkill(CharId, SkillId, job);
        }




        void IDatabase.LoadSkills(Character character, uint CharId)
        {
            LoadSkills(character, CharId);
        }


        bool IDatabase.MarkAsReadMailItem(uint id)
        {
            return MarkAsReadMailItem(id);
        }

        bool IDatabase.QuestComplete(uint charId, uint QuestId)
        {
            return QuestComplete(charId, QuestId);
        }

        uint IDatabase.RegisterNewMarketItem(Character target, MarketItemArgument arg)
        {
            return RegisterNewMarketItem(target, arg);
        } 


        IEnumerable<MarketItemArgument> IDatabase.SearchMarket(Saga.Map.Definitions.Misc.MarketSearchArgument Argument)
        {
            return SearchMarket(Argument);
        }

        IEnumerable<MarketItemArgument> IDatabase.SearchMarketForOwner(Character target)
        {
            return SearchMarketForOwner(target);
        }

        uint IDatabase.UnregisterMarketItem(uint id)
        {
            return UnregisterMarketItem(id);
        }

        uint IDatabase.UpdateCommentByPlayerId(uint id, string message)
        {
            return UpdateCommentByPlayerId(id, message);
        }

        bool IDatabase.UpdateSkill(Character target, uint SkillId, uint Experience)
        {
            return UpdateSkill(target, SkillId, Experience);
        }

        bool IDatabase.UpdateZenyAttachment(uint Id, uint Zeny)
        {
            return UpdateZenyAttachment(Id, Zeny);
        }

        bool IDatabase.UpgradeSkill(Character target, uint OldSkillId, uint NewSkillId, uint Experience)
        {
            return UpgradeSkill(target, OldSkillId, NewSkillId, Experience);
        }

        bool IDatabase.VerifyNameExists(string name)
        {
            return VerifyNameExists(name);
        }

        Rag2Item IDatabase.GetItemAttachment(uint MailId)
        {
            return GetItemAttachment(MailId);
        }

        bool IDatabase.UpdateItemAttachment(uint MailId, Rag2Item Attachment)
        {
            return UpdateItemAttachment(MailId, Attachment);
        }

        bool IDatabase.DeleteEventItemId(uint RewardId)
        {
            return DeleteEventItemId(RewardId);
        }

        bool IDatabase.CreateEventItem(uint CharId, uint ItemId, byte Stack)
        {
            return CreateEventItem(CharId, ItemId, Stack);
        }

        bool IDatabase.GetPlayerId(string name, out uint charid)
        {
            return GetPlayerId(name, out charid);
        }

        EventItem IDatabase.FindEventItemById(Character target, uint RewardId)
        {
            return FindEventItemById(target, RewardId);
        }

        System.Collections.ObjectModel.Collection<EventItem> IDatabase.FindEventItemList(Character target)
        {
            return FindEventItemList(target);
        }



        #endregion

        #region IDatabase Query Provider

        System.Data.IDataReader IDatabase.ExecuteDataReader(Saga.Data.IQueryProvider query, System.Data.CommandBehavior behavior)
        {
            return ExecuteDataReader(query, behavior);
        }

        System.Data.IDataReader IDatabase.ExecuteDataReader(Saga.Data.IQueryProvider query)
        {
            return ExecuteDataReader(query);
        }

        int IDatabase.ExecuteNonQuery(Saga.Data.IQueryProvider query)
        {
            return ExecuteNonQuery(query);
        }

        #endregion



        #region IDatabase Members Characters

        #endregion

        #region IDatabase Members Additions

       
        #endregion

        #region IDatabase Members Inventory

     

        #endregion

        #region IDatabase Members Storage

        #endregion

   

     

        #region IDatabase Members Equipment

        #endregion

        #region IDatabase Members General

        bool IDatabase.CheckDatabaseFields()
        {
            MySqlConnection connection = ConnectionPool.Request();
            try
            {
                return CheckDatabaseFields(connection);
            }
            finally
            {
                ConnectionPool.Release(connection);
            }
        }

        bool IDatabase.TransactionSave(IInfoProvider2 dbq)
        {
            return TransactionSave(dbq);
        }

        bool IDatabase.TransactionLoad(IInfoProvider2 dbq, bool continueOnError)
        {
            return TransactionLoad(dbq, continueOnError);
        }

        bool IDatabase.TransactionRepair(IInfoProvider2 dbq)
        {
            return TransactionRepair(dbq);
        }

        bool IDatabase.TransactionInsert(IInfoProvider2 dbq)
        {
            return TransactionInsert(dbq);
        }

        bool IDatabase.GetCharacterId(string name, out uint charId)
        {
            return GetCharacterId(name, out charId);
        }

        #endregion

        #region Private Members

        private TraceLog __dbtracelog = new TraceLog("Database", "Trace switch for the database plugin",3);
        private bool __canUseStoredProcedures = false;    

        #endregion

        #region Private Constant Members

        const string _query_01 = "SELECT `list_quests`.`QuestId`, `list_quests`.`NPC` FROM `list_quests` WHERE `list_quests`.`QuestId` != ?Quest AND `list_quests`.`QuestType`=2 AND `list_quests`.`Req_Clvl` <= ?Clvl AND `list_quests`.`Req_Jlvl` <= ?Jlvl AND `list_quests`.`NPC` > 0 AND `list_quests`.`QuestId` NOT IN ( SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=?CharId ) AND (`list_quests`.`Req_Quest`='0' OR `list_quests`.`Req_Quest` IN (SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=?CharId)) GROUP BY `list_quests`.`NPC` LIMIT 50;";
        const string _query_02 = "SELECT * FROM `list_quests` WHERE `list_quests`.`NPC`=?Modelid AND `list_quests`.`QuestType`=1 AND `list_quests`.`Req_Clvl` <= ?Clvl AND `list_quests`.`Req_Jlvl` <= ?Jlvl AND `list_quests`.`QuestId` NOT IN ( SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=?CharId ) AND ( `list_quests`.`Req_Quest`='0' OR `list_quests`.`Req_Quest` IN (SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=?CharId)) LIMIT 30;";
        const string _query_03 = "SELECT * FROM `list_additions` WHERE `CharId`=?CharId";
        const string _query_04 = "UPDATE `list_additions` SET `Additions`=?Additions WHERE `CharId`=?CharId";
        const string _query_05 = "INSERT INTO `list_additions` (`CharId`,`Additions`) VALUES (?CharId, ?Additions);";
        const string _query_06 = "DELETE FROM `auction` WHERE `AuctionId`=?AuctionId LIMIT 1";
        const string _query_07 = "SELECT COUNT(*) FROM `auction` WHERE `CharName`=?CharName";
        const string _query_08 = "SELECT * FROM `auction` WHERE `AuctionId`=?AuctionId'";
        const string _query_09 = "SELECT * FROM `auction` WHERE CharId=?CharId";
        const string _query_10 = "INSERT INTO `auction` (`Categorie`, `Grade`, `CharId`, `CharName`,`ItemName`, `ReqClvl`,`Price`,`ItemContent`,`Expires`) VALUES (?Categorie, ?Grade, ?CharId, ?CharName, ?ItemName, ?ReqClvl,?Price,?ItemContent, ?Expires)";
        const string _query_11 = "DELETE FROM `auction` WHERE `AuctionId`=?AuctionId";
        const string _query_12 = "SELECT * FROM `characters` WHERE `CharId`=?CharId LIMIT 1";
        const string _query_13 = "UPDATE `characters` SET `Cexp`=?Cexp, `Jexp`=?Jexp, `Job`=?Job, `Map`=?Map, `HP`=?HP, `SP`=?SP, LP=?LP, LC=?LC, `Position.x`=?Posx, `Position.y`=?Posy, `Position.z`=?Posz, `Saveposition.x`=?Savex, `Saveposition.y`=?Savey, `Saveposition.z`=?Savez, `Saveposition.map`=?Savemap, `Stats.Str`=?Str, `Stats.Dex`=?Dex, `Stats.Int`=?Int, `Stats.Con`=?Con, `Stats.Luc`=?Luc, `Stats.Pending`=?Pending, `Rufi`=?Rufi WHERE `CharId`=?CharId LIMIT 1";
        const string _query_14 = "SELECT `Equipement` FROM `list_equipment` WHERE `CharId`=?CharId";
        const string _query_15 = "UPDATE `list_equipment` SET `Equipement`=?Equipment WHERE `CharId`=?CharId;";
        const string _query_16 = "INSERT `list_equipment` (`CharId`,`Equipement`) VALUES (?CharId,?Equipment);";
        const string _query_17 = "SELECT `CharId` FROM `characters` WHERE `CharName`=?CharName";
        const string _query_18 = "DELETE FROM `list_eventrewards` WHERE `RewardId`=?RewardId;";
        const string _query_19 = "INSERT INTO `list_eventrewards` (`CharId`,`ItemId`,`ItemCount`) VALUES (?CharId,?ItemId,?ItemCount)";
        const string _query_20 = "SELECT * FROM `list_eventrewards` WHERE `CharId`=?CharId AND RewardId=?RewardId;";
        const string _query_21 = "SELECT * FROM `list_eventrewards` WHERE `CharId`=?CharId;";
        const string _query_22 = "SELECT `CharId` FROM characters ORDER BY `CharName`;";
        const string _query_23 = "SELECT `FriendName` FROM `list_friends` WHERE CharId=?CharId LIMIT 20;";
        const string _query_24 = "SELECT `FriendName`,`Reason` FROM `list_blacklist` WHERE CharId=?CharId LIMIT 10;";
        const string _query_25 = "INSERT INTO `list_friends` (`CharId`,`FriendName`) VALUES (?CharId, ?FriendName);";
        const string _query_26 = "DELETE FROM `list_friends` WHERE `CharId`=?CharId AND `FriendName`=?FriendName LIMIT 1;";
        const string _query_27 = "INSERT INTO `list_blacklist` (`CharId`,`FriendName`,`Reason`) VALUES (?CharId, ?FriendName, ?Reason);";
        const string _query_28 = "DELETE FROM `list_blacklist` WHERE `CharId`=?CharId AND `FriendName`=?FriendName LIMIT 1;";
        const string _query_29 = "SELECT `ContainerMaxStorage`,`Container` FROM `list_inventory` WHERE `CharId`=?CharId";
        const string _query_30 = "UPDATE `list_inventory` SET `ContainerMaxStorage`=?ContainerMaxStorage, `Container`=?Container WHERE `CharId`=?CharId";
        const string _query_31 = "INSERT INTO `list_inventory` (`CharId`,`ContainerMaxStorage`,`Container`) VALUES (?CharId, ?MaxStorage, ?Container);";
        const string _query_32 = "SELECT `JobInformation` FROM `list_joblinformation` WHERE `CharId`=?CharId";
        const string _query_33 = "UPDATE `list_joblinformation` SET `JobInformation`=?JobInformation WHERE CharId=?CharId;";
        const string _query_34 = "INSERT INTO `list_joblinformation` (`CharId`,`JobInformation`) VALUES (?CharId, ?JobInformation);";
        const string _query_35 = "SELECT * FROM list_maildata WHERE Receiptent=?Receiptent AND IsInbox=1;";
        const string _query_36 = "UPDATE list_maildata SET IsChecked=1 WHERE Receiptent=?Receiptent AND IsInbox=1;";
        const string _query_37 = "SELECT * FROM list_maildata WHERE Sender=?Sender AND IsOutbox=1 AND IsRead=0;";
        const string _query_38 = "SELECT COUNT(*) FROM list_maildata WHERE Receiptent=?Receiptent AND (IsInbox=1 OR IsPending=1);";
        const string _query_39 = "SELECT COUNT(*) FROM list_maildata WHERE Receiptent=?Receiptent AND IsInbox=1 AND IsChecked=0";
        const string _query_40 = "SELECT COUNT(*) FROM list_maildata WHERE Sender=?Sender AND IsOutbox=1 AND IsRead=0";
        const string _query_41 = "SELECT Zeny FROM list_maildata WHERE MailId=?Id;";
        const string _query_42 = "UPDATE list_maildata SET Zeny=?Zeny WHERE MailId=?Id";
        const string _query_43 = "SELECT Attachment FROM list_maildata WHERE MailId=?Id;";
        const string _query_44 = "UPDATE list_maildata SET Attachment=?Attachment WHERE MailId=?Id";
        const string _query_45 = "INSERT INTO list_maildata ( Sender, Receiptent, Date, Topic, Message, Attachment, Zeny ) VALUES ( ?Sender, ?Receiptent, ?Date, ?Topic, ?Message, ?Attachment, ?Zeny);";
        const string _query_46 = "UPDATE list_maildata SET IsRead=1, IsOutbox=0, DateRead=NOW() WHERE MailId=?Id";
        const string _query_47 = "SELECT * FROM list_maildata WHERE MailId=?Id LIMIT 1;";
        const string _query_48 = "SELECT Receiptent, COUNT(MailId) FROM list_maildata WHERE IsPending=1 GROUP BY Receiptent LIMIT 300;";
        const string _query_49 = "UPDATE list_maildata SET IsPending=0, IsInbox=1 WHERE Receiptent=?Receiptent AND IsPending=1";
        const string _query_50 = "UPDATE list_maildata SET IsInbox=0 WHERE MailId=?Id";
        const string _query_51 = "UPDATE list_maildata SET IsOutbox=0, IsPending=0 WHERE MailId=?Id";
        const string _query_52 = "SELECT COUNT(*) FROM `list_questhistory` WHERE `CharId`=?CharId AND `QuestId`=?QuestId";
        const string _query_53 = "CALL list_availablequests(?CharId,?Modelid,?Clvl,?Jlvl);";
        const string _query_54 = "CALL list_availablepersonalrequests(?CharId,?Region,?Clvl,?Jlvl,?Quest);";
        const string _query_55 = "INSERT INTO `list_questhistory` (`CharId`,`QuestId`) VALUES (?CharId, ?QuestId);";
        
        const string _query_57 = "UPDATE `list_queststates` SET `State`=?State WHERE `CharId`=?CharId";
        const string _query_58 = "UPDATE `list_queststates` SET `State`=NULL WHERE `CharId`=?CharId";
        const string _query_59 = "SELECT `State` FROM `list_queststates` WHERE CharId=?CharId LIMIT 1;";
        const string _query_60 = "SELECT `SkillId`,`SkillExp` FROM `list_learnedskills` WHERE `CharId`=?CharId";
        const string _query_61 = "SELECT `SkillId` FROM `list_learnedskills` WHERE `CharId`=?CharId GROUP BY SkillId";
        const string _query_62 = "UPDATE `list_learnedskills` SET `SkillExp`=?SkillExp WHERE `CharId`=?CharId AND `SkillId`=?SkillId;";
        const string _query_63 = "INSERT INTO `list_learnedskills` (`CharId`,`SkillId`,`SkillExp`, `Job`) VALUES (?CharId, ?SkillId, ?SkillExp, ?Job);";
        const string _query_64 = "UPDATE `list_learnedskills` SET `SkillId`=?NewSkillId, `SkillExp`=?SkillExp WHERE `CharId`=?CharId AND `Job`=?Job AND `SkillId`=?SkillId;";
        const string _query_65 = "SELECT `SkillId` FROM `list_learnedskills` WHERE `CharId`=?CharId";
        const string _query_66 = "UPDATE `list_specialskills` SET `Skills`=?Skills WHERE `CharId`=?CharId LIMIT 1";
        const string _query_67 = "SELECT `Skills` FROM `list_specialskills` WHERE `CharId`=?CharId";
        const string _query_68 = "INSERT INTO `list_weaponary` (`CharId`,`Weaponary`,`UnlockedWeaponCount`,`PrimairyWeapon`,`SecondaryWeapoin`,`ActiveWeaponIndex`) VALUES (?CharId, ?Weaponary, ?UnlockedWeaponCount,?PrimairyWeapon,?SecondaryWeapon,?ActiveWeaponIndex); ";
        const string _query_69 = "SELECT * FROM `list_weaponary` WHERE `CharId`=?CharId";
        const string _query_70 = "UPDATE `list_weaponary` SET `Weaponary`=?Weaponary, `UnlockedWeaponCount`=?UnlockedWeaponCount, `PrimairyWeapon`=?PIndex, SecondaryWeapoin=?SIndex, ActiveWeaponIndex=?AIndex WHERE `CharId`=?CharId";
        const string _query_71 = "SELECT `ZoneState` FROM `list_zoneinformation` WHERE `CharId`=?CharId";
        const string _query_72 = "UPDATE `list_zoneinformation` SET `ZoneState`=?ZoneState WHERE CharId=?CharId;";
        const string _query_73 = "INSERT INTO `list_zoneinformation` (`CharId`,`ZoneState`) VALUES (?CharId, ?ZoneState);";

        const string _query_74 = "SELECT COUNT(*) FROM `characters` WHERE `CharId`=?CharId;";
        const string _query_75 = "SELECT COUNT(*) FROM `list_additions` WHERE `CharId`=?CharId;";
        const string _query_76 = "SELECT COUNT(*) FROM `list_equipment` WHERE `CharId`=?CharId;";
        const string _query_77 = "SELECT COUNT(*) FROM `list_inventory` WHERE `CharId`=?CharId;";
        const string _query_78 = "SELECT COUNT(*) FROM `list_joblinformation` WHERE `CharId`=?CharId;";
        const string _query_79 = "SELECT COUNT(*) FROM `list_queststates` WHERE `CharId`=?CharId;";
        const string _query_80 = "SELECT COUNT(*) FROM `list_specialskills` WHERE `CharId`=?CharId;";
        const string _query_81 = "SELECT COUNT(*) FROM `list_storage` WHERE `CharId`=?CharId;";
        const string _query_82 = "SELECT COUNT(*) FROM `list_weaponary` WHERE `CharId`=?CharId;";
        const string _query_83 = "SELECT COUNT(*) FROM `list_zoneinformation` WHERE `CharId`=?CharId;";

        const string _query_84 = "INSERT INTO `characters` (`UserId`,`CharName`,`UppercasedCharName`,`CharFace`,`CharId`,`Cexp`, `Jexp`, `Job`, `Map`, `HP`, `SP`, LP, LC, `Position.x`, `Position.y`, `Position.z`, `Saveposition.x`, `Saveposition.y`, `Saveposition.z`, `Saveposition.map`, `Stats.Str`, `Stats.Dex`, `Stats.Int`, `Stats.Con`, `Stats.Luc`, `Stats.Pending`, `Rufi`) VALUES (?UserId,?CharName,?UppercasedCharName,?CharFace,?CharId, ?Cexp, ?Jexp, ?Job, ?Map, ?HP, ?SP,?LP, ?LC, ?Posx, ?Posy, ?Posz, ?Savex, ?Savey, ?Savez, ?Savemap, ?Str, ?Dex, ?Int, ?Con, ?Luc, ?Pending, ?Rufi);";
        const string _query_85 = "INSERT INTO `characters` (`UserId`,`CharName`,`UppercasedCharName`,`CharFace`,`Cexp`, `Jexp`, `Job`, `Map`, `HP`, `SP`, LP, LC, `Position.x`, `Position.y`, `Position.z`, `Saveposition.x`, `Saveposition.y`, `Saveposition.z`, `Saveposition.map`, `Stats.Str`, `Stats.Dex`, `Stats.Int`, `Stats.Con`, `Stats.Luc`, `Stats.Pending`, `Rufi`) VALUES (?UserId,?CharName,?UppercasedCharName,?CharFace,?Cexp, ?Jexp, ?Job, ?Map, ?HP, ?SP,?LP, ?LC, ?Posx, ?Posy, ?Posz, ?Savex, ?Savey, ?Savez, ?Savemap, ?Str, ?Dex, ?Int, ?Con, ?Luc, ?Pending, ?Rufi);";
        const string _query_86 = "INSERT `list_storage` (`CharId`,`ContainerMaxStorage`,`Container`) VALUES (?CharId, ?ContainerMaxStorage,?Container)";


        const string _query_56 = "INSERT INTO `list_queststates` (`CharID`,`State`) VALUES (?CharId, NULL);";
        const string _query_87 = "INSERT INTO `list_queststates` (`CharID`,`State`) VALUES (?CharId, ?State);";
        const string _query_88 = "INSERT INTO `list_specialskills` (`CharId`,`Skills`) VALUES (?CharId, ?Skills);";
        const string _query_89 = "SELECT `CharId` FROM `characters` WHERE `UppercasedCharName`=?UppercasedCharName;";        

        #endregion


        bool TransactionSave(IInfoProvider2 dbq)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = ConnectionPool.Request();
                transaction = connection.BeginTransaction();
                SaveCharacterEx(connection, dbq);       //save character information
                SaveAdditionsEx(connection, dbq);       //save addition information
                SaveEquipmentEx(connection, dbq);       //save equipment information
                SaveInventoryEx(connection, dbq);       //save inventory information
                SaveStorageEx(connection, dbq);         //save storage informatiom
                SaveJobinformationEx(connection, dbq);  //Save job information
                SaveWeaponsEx(connection, dbq);         //Save weapon information
                SaveZoneEx(connection, dbq);            //Save zone information
                SaveQuestEx(connection, dbq);           //Save quest information
                SaveSpecialSkillsEx(connection, dbq);   //Save special skill information                               
                                              
                transaction.Commit();
                return true;
            }
            catch (MySqlException ex)
            {
                __dbtracelog.WriteError("database", "Transaction save failed {0}", ex);

                try
                {
                    transaction.Rollback();
                }
                catch (MySqlException)
                {
                    __dbtracelog.WriteError("database", "Transaction failed to roll back");
                }

                return false;
            }
            catch (Exception ex)
            {
                __dbtracelog.WriteError("database", "Transaction save failed {0}", ex);
                return false;
            }
            finally
            {
                ConnectionPool.Release(connection);     //Always release connection
            }
        }
        bool TransactionLoad(IInfoProvider2 dbq, bool continueOnError)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = ConnectionPool.Request();
                transaction = connection.BeginTransaction();
                bool success = LoadCharacterEx(connection, dbq)                       //Load character information
                             & LoadAdditionsEx(connection, dbq, continueOnError)      //Load addition information
                             & LoadEquipmentEx(connection, dbq, continueOnError)      //Load equipment information
                             & LoadInventoryEx(connection, dbq, continueOnError)      //Load inventory information
                             & LoadStorageEx(connection, dbq, continueOnError)        //Load storage information
                             & LoadJobinformationEx(connection, dbq, continueOnError) //Load job information
                             & LoadWeaponsEx(connection, dbq, continueOnError)        //Load weapon information
                             & LoadZoneEx(connection, dbq, continueOnError)           //Load zone information
                             & LoadQuestEx(connection, dbq, continueOnError)          //Load quest information
                             & LoadSkillsEx(connection, dbq, continueOnError)         //Load normal skills
                             & LoadSpecialSkillsEx(connection, dbq, continueOnError)  //Load special skill information
                             & LoadFriendlistEx(connection, dbq)                      //Load friendlist information
                             & LoadBlacklistEx(connection, dbq);                      //Load blacklist information

                transaction.Commit();
                return success;
            }
            catch (MySqlException ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);

                try
                {
                    transaction.Rollback();
                }
                catch (MySqlException)
                {
                    __dbtracelog.WriteError("database", "Transaction failed to roll back");                    
                }

                return false;
            }
            catch (Exception ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);
                return false;
            }
            finally
            {
                ConnectionPool.Release(connection);     //Always release connection
            }
        }
        bool TransactionRepair(IInfoProvider2 dbq)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = ConnectionPool.Request();
                transaction = connection.BeginTransaction();

                if (!ExistsCharacterEx(connection, dbq)) InsertCharacterEx(connection, dbq);
                if (!ExistsAdditionsEx(connection, dbq)) InsertAdditionsEx(connection, dbq);
                if (!ExistsEquipmentEx(connection, dbq)) InsertEquipmentEx(connection, dbq);
                if (!ExistsInventoryEx(connection, dbq)) InsertInventoryEx(connection, dbq);
                if (!ExistsStorageEx(connection, dbq)) InsertStorageEx(connection, dbq);
                if (!ExistsJobinformationEx(connection, dbq)) InsertJobinformationEx(connection, dbq);
                if (!ExistsWeaponsEx(connection, dbq)) InsertWeaponsEx(connection, dbq);
                if (!ExistsZoneEx(connection, dbq)) InsertZoneEx(connection, dbq);
                if (!ExistsQuestEx(connection, dbq)) InsertQuestEx(connection, dbq);
                if (!ExistsSpecialSkillsEx(connection, dbq)) InsertSpecialSkillsEx(connection, dbq);

                transaction.Commit();
                return true;
            }
            catch (MySqlException ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);

                try
                {
                    transaction.Rollback();
                }
                catch (MySqlException)
                {
                    __dbtracelog.WriteError("database", "Transaction failed to roll back");
                }

                return false;
            }
            catch (Exception ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);
                return false;
            }
            finally
            {
                ConnectionPool.Release(connection);     //Always release connection
            }

        }

        bool TransactionInsert(IInfoProvider2 dbq)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = ConnectionPool.Request();
                transaction = connection.BeginTransaction();

                bool success = InsertCharacterEx(connection, dbq)
                             & InsertAdditionsEx(connection, dbq)
                             & InsertEquipmentEx(connection, dbq)
                             & InsertInventoryEx(connection, dbq)
                             & InsertStorageEx(connection, dbq)
                             & InsertJobinformationEx(connection, dbq)
                             & InsertWeaponsEx(connection, dbq)
                             & InsertZoneEx(connection, dbq)
                             & InsertQuestEx(connection, dbq)
                             & InsertSpecialSkillsEx(connection, dbq);

                transaction.Commit();
                return success;
            }
            catch (MySqlException ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);

                try
                {
                    transaction.Rollback();
                }
                catch (MySqlException)
                {
                    __dbtracelog.WriteError("database", "Transaction failed to roll back");
                }

                return false;
            }
            catch (Exception ex)
            {
                __dbtracelog.WriteError("database", "Transaction load failed {0}", ex);
                return false;
            }
            finally
            {
                ConnectionPool.Release(connection);     //Always release connection
            }

        }


    }
}
