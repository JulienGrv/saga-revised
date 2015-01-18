using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Authentication.Utils.Definitions.Misc;
using Saga.Data;
using System.Net;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Saga.Authentication.Data.Mysql
{
    public partial class MysqlProvider :IDatabase
    {
        #region IDatabase Members

        bool IDatabase.CheckServerVersion()
        {
            return CheckServerVersion();
        }

        bool IDatabase.Connect(ConnectionInfo info)
        {
            return Connect(info);
        }

        bool IDatabase.CreateLoginEntry(string username, string password, byte gender, int gmlevel)
        {
            return CreateLoginEntry(username, password, gender, gmlevel);
        }

        bool IDatabase.GetCharacterWorldCount(uint userid, out Dictionary<byte, byte> CharPerWorld)
        {
            return GetCharacterWorldCount(userid, out CharPerWorld);
        }

        bool IDatabase.Login(string username, out LoginResult result)
        {
            return Login(username, out result);
        }

        bool IDatabase.RemoveCharacterOnWorld(uint userid, uint worldid)
        {
            return RemoveCharacterOnWorld(userid, worldid);
        }

        bool IDatabase.UpdateCharacterOnWorld(uint userid, uint worldid)
        {
            return UpdateCharacterOnWorld(userid, worldid);
        }

        bool IDatabase.UpdateLoginEntry(uint AccountId, IPAddress address)
        {
            return UpdateLoginEntry(AccountId, address);
        }
        
        List<Saga.Authentication.Structures.WorldInfo> IDatabase.GetWorldInformation()
        {
            return GetWorldInformation();
        }

        bool IDatabase.UpdateLastplayedWorld(uint AccountId, byte world)
        {
            return UpdateLastplayedWorld(AccountId, world);
        }

        bool IDatabase.UpdateSession(uint AccountId, uint sessionid)
        {
            return UpdateSession(AccountId, sessionid);
        }

        bool IDatabase.ReleaseSessionId(uint sessionid)
        {
            return ReleaseSessionId(sessionid);
        }

        int IDatabase.ClearSession(byte worldid)
        {
            return ClearSession(worldid);
        }

        List<AclEntry> IDatabase.ListAclEntries()
        {
            return ListAclEntries();
        }

        bool IDatabase.IsAllowed(System.Net.IPAddress adress)
        {
            return IsAllowed(adress);
        }

        bool IDatabase.IsDenied(System.Net.IPAddress adress)
        {
            return IsDenied(adress);
        }

        bool IDatabase.AddAclEntry(string ip, string mask, byte operation)
        {
            return AddAclEntry(ip, mask, operation);
        }

        bool IDatabase.RemoveAclEntry(string ip, string mask, byte operation)
        {
            return RemoveAclEntry(ip, mask, operation);
        }

        List<AclEntry> IDatabase.FindAclEntries(string ip)
        {
            return FindAclEntries(ip);
        }

        long IDatabase.GetGameTime(uint accountid)
        {
            return GetGameTime(accountid);
        }

        IPAddress IDatabase.GetIpAddress(uint accountid)
        {
            return GetIpAddress(accountid);
        }

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

        #endregion

        #region IDatabase Members


        bool IDatabase.AddWorld(string worldname, string password, out int worldid)
        {
            return AddWorld(worldname, password, out worldid);
        }

        bool IDatabase.RemoveWorld(string worldname)
        {
            return RemoveWorld(worldname);
        }

        bool IDatabase.RenameWorld(string worldname, string newworldname)
        {
            return RenameWorld(worldname, newworldname);
        }

        List<string> IDatabase.GetAllCharactersOnline()
        {
            return GetAllCharactersOnline();
        }

        #endregion

        #region Private Members

        private TraceLog __dbtracelog = new TraceLog("Database", "Trace switch for the database plugin", 3);       

        #endregion

        #region Private Constant Members

        const string _query_01 = "SELECT * FROM `login` WHERE `Username`=?Username LIMIT 1";
        const string _query_02 = "UPDATE `login` SET `LastLoginDate`=NOW(), LastUserIP=INET_ATON(?IP) WHERE `UserId`=?UserId LIMIT 1";
        const string _query_03 = "INSERT INTO `login` (`Username`,`Password`,`Gender`, `LastUserIP`,`LastSession`,`ActiveSession`,`LastLoginDate`,`IsActivated`, `IsBanned`, `HasAgreed`, `DateOfBirth`, `DateOfRegistration`, `IsTestAccount`,`GmLevel` ) VALUES (?Username,MD5(?Password),?Gender, 0, 0, 0, ?DateOfLastLogin,1, 0, 1, ?DateOfBirth, ?DateOfRegistration, 0, ?GmLevel);";
        const string _query_04 = "UPDATE `login` SET `IsBanned`=1 WHERE `Username`=?Username LIMIT 1;";
        const string _query_05 = "SELECT * FROM `list_worldcharacters` WHERE `UserId`=?UserId";
        const string _query_06 = "UPDATE `list_worldcharacters` SET `CharacterCount`= `CharacterCount` + 1 WHERE `UserId`=?UserId AND WorldId=?WorldId";
        const string _query_07 = "INSERT INTO `list_worldcharacters` (`UserId`,`WorldId`,`CharacterCount`) VALUES (?UserId,?WorldId,1);";
        const string _query_08 = "UPDATE `list_worldcharacters` SET `CharacterCount`= `CharacterCount` - 1 WHERE `UserId`=?UserId AND WorldId=?WorldId";
        const string _query_09 = "INSERT INTO list_worlds (`Name`,`Proof`) VALUES (?Name, ?Proof);";
        const string _query_10 = "SELECT COUNT(*) FROM list_worlds WHERE `Name`=?Name;";
        const string _query_11 = "DELETE FROM list_worlds WHERE `Name`=?Name;";
        const string _query_12 = "UPDATE list_worlds SET `Name`=?NewName WHERE `Name`=?OldName;";
        const string _query_13 = "SELECT COUNT(*) FROM list_worlds WHERE `Name`=?Name;";
        const string _query_14 = "SELECT RuleId, INET_NTOA(`FilterIp`) AS `FilterIp`,	INET_NTOA(`Mask`) AS `Mask`, Operation FROM `list_acl` WHERE ((FilterIp & Mask) = (INET_ATON(?IP) & Mask));";
        const string _query_15 = "DELETE FROM `list_acl` WHERE `Operation`=?Op AND FilterIp=INET_ATON(?FilterIp) AND Mask=INET_ATON(?Mask);";
        const string _query_16 = "SELECT COUNT(*) FROM `list_acl` WHERE `Operation`=?Op AND FilterIp=INET_ATON(?FilterIp) AND Mask=INET_ATON(?Mask);";
        const string _query_17 = "INSERT INTO`list_acl` (`Operation`,`FilterIp`,`Mask`) VALUES (?Op,INET_ATON(?FilterIp),INET_ATON(?Mask));";
        const string _query_18 = "SELECT COUNT(*) FROM `list_acl` WHERE `Operation`='1' AND ((FilterIp & Mask) = (INET_ATON(?IP) & Mask));";
        const string _query_19 = "SELECT COUNT(*) FROM `list_acl` WHERE `Operation`='0' AND ((FilterIp & Mask) = (INET_ATON(?IP) & Mask));";
        const string _query_20 = "SELECT RuleId, INET_NTOA(`FilterIp`) AS `FilterIp`,	INET_NTOA(`Mask`) AS `Mask`, Operation FROM  `list_acl`;";
        const string _query_21 = "SELECT * FROM `list_worlds`";
        const string _query_22 = "UPDATE `login` SET `LastPlayedWorld`= ?LastPlayedWorld WHERE `UserId`=?UserId";
        const string _query_23 = "UPDATE `login` SET `ActiveSession`= ?ActiveSession WHERE `UserId`=?UserId";
        const string _query_24 = "UPDATE `login` SET `ActiveSession`=0, `GameTime` =  `GameTime` + (UNIX_TIMESTAMP(NOW()) - UNIX_TIMESTAMP(`LastLoginDate`)) WHERE `ActiveSession`=?ActiveSession";
        const string _query_25 = "UPDATE `login` SET `ActiveSession`=0 WHERE `ActiveSession`>0 AND `LastPlayedWorld`=?LastPlayedWorld";
        const string _query_26 = "SELECT `GameTime` FROM `login` WHERE `UserId`=?UserId";
        const string _query_27 = "SELECT INET_NTOA(`LastUserIP`) FROM `login` WHERE `UserId`=?UserId";
        const string _query_28 = "SELECT `Username` FROM `login` WHERE `ActiveSession`>0";

        #endregion
    }
}
