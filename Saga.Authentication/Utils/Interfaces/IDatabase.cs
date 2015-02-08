using Saga.Authentication.Structures;
using Saga.Data;
using System.Collections.Generic;
using System.Net;

namespace Saga.Authentication.Utils.Definitions.Misc
{
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
        /// <param name="character"></param>
        /// <param name="CharId"></param>
        bool Connect(ConnectionInfo info);

        /// <summary>
        /// Checks the database for missing tables and fields
        /// </summary>
        bool CheckDatabaseFields();

        #endregion General

        bool Login(string username, out LoginResult result);

        bool GetCharacterWorldCount(uint userid, out Dictionary<byte, byte> CharPerWorld);

        bool UpdateCharacterOnWorld(uint userid, uint worldid);

        bool RemoveCharacterOnWorld(uint userid, uint worldid);

        bool UpdateLoginEntry(uint AccountId, IPAddress address);

        bool CreateLoginEntry(string username, string password, byte gender, int gmlevel);

        bool UpdateSession(uint AccountId, uint sessionid);

        bool UpdateLastplayedWorld(uint AccountId, byte world);

        bool ReleaseSessionId(uint sessionid);

        int ClearSession(byte worldid);

        List<string> GetAllCharactersOnline();

        List<AclEntry> ListAclEntries();

        List<AclEntry> FindAclEntries(string ip);

        bool IsAllowed(IPAddress adress);

        bool IsDenied(IPAddress adress);

        bool AddAclEntry(string ip, string mask, byte operation);

        bool RemoveAclEntry(string ip, string mask, byte operation);

        List<WorldInfo> GetWorldInformation();

        bool AddWorld(string worldname, string password, out int worldid);

        bool RemoveWorld(string worldname);

        bool RenameWorld(string worldname, string newworldname);

        IPAddress GetIpAddress(uint accountid);

        long GetGameTime(uint accountid);
    }
}